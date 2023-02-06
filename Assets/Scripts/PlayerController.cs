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
    [SerializeField] LeftButton LeftButtonPrefab;
    [SerializeField] RightButton RightButtonPrefab;
    public bool buttonActive;

    private float step;
    Transform playerTransform;
    Transform targetTransform;
    private readonly float SPEED = 1;
    private bool movingToMiddle;
    private Animator animator;

    void Awake()
    {
        isSet = false;
        player = PhotonNetwork.LocalPlayer;
        PV = GetComponent<PhotonView>();
        buttonActive = false;
        step = SPEED * Time.deltaTime;
        playerTransform = gameObject.transform;
        targetTransform = ReferenceManager.Instance.middle.transform;
        movingToMiddle = false;
        animator = gameObject.GetComponent<Animator>();
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
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        return;
                    }
                    if (!hitPV.IsMine)
                    {
                        LeftButton leftButtonTemp;
                        RightButton rightButtonTemp;
                        leftButtonTemp = Instantiate(LeftButtonPrefab, hitPV.transform.position, Quaternion.identity);
                        rightButtonTemp = Instantiate(RightButtonPrefab, hitPV.transform.position, Quaternion.identity);

                        leftButtonTemp.house = hitPV.GetComponent<HouseController>();
                        leftButtonTemp.owner = hitPV.Owner;

                        rightButtonTemp.house = hitPV.GetComponent<HouseController>();
                        rightButtonTemp.owner = hitPV.Owner;

                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (movingToMiddle)
        {
            moveToMiddle();
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

        GameObject model = null;

        switch (role)
        {
            case "VILLAGER":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Base"), transform.position, Quaternion.identity, 0, data);
                break;

            case "DOCTOR":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Doctor"), transform.position, Quaternion.identity, 0, data);
                break;

            case "MAFIA":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mafia"), transform.position, Quaternion.identity, 0, data);
                break;

            case "DETECTIVE":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Detective"), transform.position, Quaternion.identity, 0, data);
                break;

            default:
                Debug.Log("MESH ERROR");
                break;
        }
        GameManager.Instance.activateDisplayRole(role);

    }
    public IEnumerator dieSequence()
    {
        yield return new WaitForSeconds(1f);

        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", true);

        movingToMiddle = true;

        yield return new WaitForSeconds(3f);

        dieAnimation();

        yield return new WaitForSeconds(4f);
    }

    public IEnumerator accusedSequence()
    {
        yield return new WaitForSeconds(1f);

        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", true);

        movingToMiddle = true;
    }

    public IEnumerator guiltySequence()
    {
        dieAnimation();

        yield return new WaitForSeconds(4f);
    }

    private void moveToMiddle()
    {
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetTransform.position, step);

        if (Vector3.Distance(playerTransform.position, targetTransform.position) < 0.001f)
        {
            movingToMiddle = false;
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
        }
    }

    private void dieAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isDead", true);
    }

}
