using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private bool isSet;
    Player player;
    public PhotonView PV;
    public bool buttonActive;

    private float step;
    public Transform playerTransform;
    public Transform middleTransform;
    private readonly float SPEED = 2;
    private bool isMovingTo;
    private Transform moveTarget;

    private HouseController insideOf;
    public Animator animator;

    void Awake()
    {
        isSet = false;
        player = PhotonNetwork.LocalPlayer;
        PV = GetComponent<PhotonView>();
        buttonActive = false;
        step = SPEED * Time.fixedDeltaTime;
        playerTransform = gameObject.transform;
        middleTransform = ReferenceManager.Instance.middle.transform;
        isMovingTo = false;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    PhotonView hitPV = hit.transform.GetComponent<PhotonView>();
                    if (hitPV)
                    {
                        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                        {
                            gameObject.SetActive(false);
                        }

                        if (!hitPV.IsMine && hit.transform.tag == "House")
                        {
                            HouseController controller = hitPV.GetComponent<HouseController>();
                            controller.showButton();
                        }
                    }
                    else
                    {
                        return;
                    }

                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isMovingTo)
        {
            move();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (PhotonNetwork.IsMasterClient)
        {
            if (changedProps["ROLE"] != null)
            {
                PV.RPC("RPC_OnSetRole", targetPlayer, changedProps["ROLE"], targetPlayer.NickName);
            }
        }
    }

    [PunRPC]
    void RPC_OnSetRole(string role, string targetName)
    {
        object[] data = { (FindObjectsOfType<PlayerController>().FirstOrDefault(x => x.PV.Owner.NickName == targetName)).GetComponent<PhotonView>().ViewID };

        GameObject model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Base"), transform.position, Quaternion.identity, 0, data);
        GameManager.Instance.activateDisplayRole(role);

    }
    private void OnTriggerEnter(Collider collider)
    {
        transform.position += new Vector3(0, .25f, 0);
    }

    private void OnTriggerExit(Collider collider)
    {
        transform.position += new Vector3(0, -.25f, 0);

    }

    public IEnumerator dieSequence()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(nameof(moveTo), middleTransform);

        yield return StartCoroutine(nameof(dieAnimation));

        yield return new WaitForSeconds(2f);

    }

    public IEnumerator accusedSequence()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(nameof(moveTo), middleTransform);
    }

    public IEnumerator guiltySequence()
    {
        yield return StartCoroutine(nameof(dieAnimation));

        yield return new WaitForSeconds(2f);
    }

    public void enterHouseSequence(int houseControllerID)
    {
        PV.RPC(nameof(RPC_enterHouseSequence), RpcTarget.All, houseControllerID);
    }

    [PunRPC]
    public IEnumerator RPC_enterHouseSequence(int houseControllerID)
    {
        PhotonView housePV = PhotonView.Find(houseControllerID);
        HouseController houseController = housePV.GetComponent<HouseController>();
        Vector3 tempScale = gameObject.transform.localScale;
        Debug.Log("OUTSIDER COUNT: " + housePV.Owner.CustomProperties["OUTSIDER_COUNT"]);

        if (PV.IsMine)
        {
            CustomPropertyWrapper.incrementProperty(housePV.Owner, "OUTSIDER_COUNT", 1);
        }
        if (!PV.IsMine)
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
        }

        transform.position = houseController.houseFront.position;
        yield return StartCoroutine(nameof(moveTo), houseController.ownerFront);

        gameObject.transform.localScale = tempScale;

        Transform outsiderTargetLocation = houseController.outsiderLocation[(int)housePV.Owner.CustomProperties["OUTSIDER_COUNT"] - 1];
        yield return StartCoroutine(nameof(moveTo), outsiderTargetLocation);

    }

    public IEnumerator moveTo(Transform target)
    {
        moveTarget = target;
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", true);

        isMovingTo = true;

        yield return new WaitUntil(() => isMovingTo == false);

        yield return new WaitForSeconds(1f);
    }

    private void move()
    {
        Debug.Log(step);
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, moveTarget.position, step);

        if (Vector3.Distance(playerTransform.position, moveTarget.position) < 0.001f)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            isMovingTo = false;
        }
    }

    private IEnumerator dieAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(2f);
    }

}
