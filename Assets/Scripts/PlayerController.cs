using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private bool isSet;
    public PhotonView PV;
    public bool buttonActive;
    private float step;
    public Transform playerTransform;
    public Transform middleTransform;
    private readonly float SPEED = 2;
    private bool isMovingTo;
    private Transform moveTarget;
    private bool isOutlined;
    private HouseController playerHouse;
    private bool disabledControls;
    private bool isMovementSync;
    private bool isSequenceRunning;
    public Animator animator;
    public PhotonAnimatorView animationSync;
    public PhotonTransformView transformSync;
    public Player isInsideOf;
    public Player previousSaved;
    public int nightSaveInterval;
    void Awake()
    {
        isSet = false;
        PV = GetComponent<PhotonView>();
        buttonActive = false;
        step = SPEED * Time.fixedDeltaTime;
        playerTransform = gameObject.transform;
        middleTransform = ReferenceManager.Instance.middle.transform;
        isMovingTo = false;
        isOutlined = false;
        playerHouse = PlayerManager.getPlayerHouseController(PV.Owner);
        transformSync = GetComponent<PhotonTransformView>();
        isInsideOf = null;
        previousSaved = PhotonNetwork.LocalPlayer;
        nightSaveInterval = 0;
        disabledControls = true;
    }

    void Update()
    {
        if (PV.IsMine && disabledControls == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PhotonView hitPV = OnClick();

                if (hitPV != null)
                {
                    if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.NIGHT)
                    {
                        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                        {
                            gameObject.SetActive(false);
                        }

                        Debug.Log(hitPV.GetComponent<Transform>().tag);
                        if (!hitPV.IsMine && hitPV.GetComponent<Transform>().tag == "House")
                        {
                            HouseController controller = hitPV.GetComponent<HouseController>();
                            controller.showButtonLeft();
                            controller.showButtonRight();
                        }
                    }
                    else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_ACCUSE)
                    {
                        Debug.Log(!hitPV.IsMine);
                        Debug.Log(hitPV.GetComponent<Transform>().tag);
                        if (!hitPV.IsMine && hitPV.GetComponent<Transform>().tag == "Player")
                        {
                            VoteManager.Instance.openAccuseVotePrompt(hitPV.Owner.NickName);
                        }
                    }
                    else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_VOTE)
                    {
                        Debug.Log(!hitPV.IsMine);
                        Debug.Log(hitPV.GetComponent<Transform>().tag);

                        if (!hitPV.IsMine && hitPV.GetComponent<Transform>().tag == "Player")
                        {
                            VoteManager.Instance.openEliminationVotePrompt();
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        if (PV.IsMine)
        {

        }
    }

    PhotonView OnClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.GetComponent<PhotonView>();
        }
        return null;
    }

    void FixedUpdate()
    {
        if (isMovingTo)
        {
            move();
        }
    }

    public void disableControls(bool state)
    {
        disabledControls = state;
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
        if (PV.IsMine)
        {
            int childrenCount = transform.childCount;

            for (int x = childrenCount - 1; x >= 0; x--)
            {
                DestroyImmediate(transform.GetChild(x).gameObject);
            }
        }


        object[] data = { (FindObjectsOfType<PlayerController>().FirstOrDefault(x => x.PV.Owner.NickName == targetName)).GetComponent<PhotonView>().ViewID };

        GameObject model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Base"), transform.position, Quaternion.identity, 0, data);
        GameManager.Instance.activateDisplayRole(role);
    }
    public void resetPlayerState()
    {
        isMovingTo = false;
        transform.position = playerHouse.ownerLocation.position;
        StartCoroutine(idleAnimation());
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

    public void enterHouseSequence(int houseControllerID, int ownerControllerID)
    {
        PV.RPC(nameof(RPC_enterHouseSequence), RpcTarget.All, houseControllerID, ownerControllerID);
    }

    [PunRPC]
    public IEnumerator RPC_enterHouseSequence(int houseControllerID, int ownerControllerID)
    {
        PhotonView housePV = PhotonView.Find(houseControllerID);
        PhotonView ownerPV = PhotonView.Find(ownerControllerID);
        HouseController houseController = housePV.GetComponent<HouseController>();
        PlayerController ownerController = ownerPV.GetComponent<PlayerController>();
        Vector3 tempScale = gameObject.transform.localScale;

        houseController.outsiderCount += 1;
        if (!PV.IsMine)
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
        }

        if (disabledControls == false)
        {
            transform.position = houseController.houseFront.position;
            yield return StartCoroutine(nameof(moveTo), houseController.ownerFront);
        }

        if (disabledControls == false)
        {
            gameObject.transform.localScale = tempScale;
            yield return StartCoroutine(greetAnimation(ownerController));
        }

        if (disabledControls == false)
        {
            Transform outsiderTargetLocation = houseController.outsiderLocation[houseController.outsiderCount - 1];
            yield return StartCoroutine(nameof(moveTo), outsiderTargetLocation);
            yield return StartCoroutine(talkAnimation(ownerController));
        }
    }

    public IEnumerator goBackToHouseSequence()
    {
        yield return StartCoroutine(nameof(moveTo), playerHouse.ownerFront);

        yield return StartCoroutine(nameof(moveTo), playerHouse.ownerLocation);
    }

    public IEnumerator moveTo(Transform target)
    {
        if (isMovementSync == true)
        {
            isSequenceRunning = true;
        }
        else
        {
            isSequenceRunning = false;
        }

        if (isMovementSync == true && PV.IsMine)
        {
            moveTarget = target;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", true);

            isMovingTo = true;

            yield return new WaitUntil(() => isMovingTo == false);

            yield return StartCoroutine(nameof(idleAnimation));

            PV.RPC(nameof(RPC_animationFinished), RpcTarget.All);
        }
        else
        {
            moveTarget = target;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", true);

            isMovingTo = true;

            yield return new WaitUntil(() => isMovingTo == false);

            yield return StartCoroutine(nameof(idleAnimation));
        }

        yield return new WaitUntil(() => isSequenceRunning == false);

        yield return new WaitForSeconds(1f);
    }

    public void setMovementSync(bool state)
    {
        if (state == true)
        {
            animationSync.enabled = true;
            transformSync.enabled = true;
            isMovementSync = true;
        }
        else
        {
            animationSync.enabled = false;
            transformSync.enabled = false;
            isMovementSync = false;
        }
    }

    private void move()
    {
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, moveTarget.position, step);

        if (Vector3.Distance(playerTransform.position, moveTarget.position) < 0.001f)
        {
            isMovingTo = false;
        }
    }

    private IEnumerator dieAnimation()
    {
        GameManager.Instance.removeFromAliveList(PV.Owner);

        isSequenceRunning = true;
        animator.SetBool("isIdle", false);
        animator.SetBool("isDead", true);

        if (PV.IsMine)
        {
            PV.RPC(nameof(RPC_animationFinished), RpcTarget.All);
        }

        yield return new WaitUntil(() => isSequenceRunning == false);

        yield return new WaitForSeconds(2f);
        disableControls(true);
    }

    private IEnumerator greetAnimation(PlayerController partnerController)
    {
        animator.SetBool("isIdle", false);
        animator.SetTrigger("Greet");

        partnerController.animator.SetBool("isIdle", false);
        partnerController.animator.SetTrigger("Greet");
        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(partnerController.idleAnimation());
    }

    private IEnumerator talkAnimation(PlayerController partnerController)
    {
        animator.SetBool("isTalking1", true);
        partnerController.animator.SetBool("isTalking2", true);
        yield return null;
    }

    private IEnumerator idleAnimation()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isTalking1", false);
        animator.SetBool("isTalking2", false);
        animator.SetBool("isIdle", true);

        yield return null;
    }

    private void OnTriggerEnter(Collider collider)
    {
        transform.position += new Vector3(0, .25f, 0);
    }

    private void OnTriggerExit(Collider collider)
    {
        transform.position += new Vector3(0, -.25f, 0);
    }
    private void OnMouseEnter()
    {
        Debug.Log("Player enter");
        if (isOutlined == false)
        {
            Color tempColor = playerHouse.houseRenderer.material.color;
            tempColor.a = .1f;
            playerHouse.houseRenderer.material.color = tempColor;

            gameObject.GetComponentInChildren<Outline>().enabled = true;
            isOutlined = true;
        }

        TooltipManager.Instance.setHoveredData("Player", PV.Owner.NickName);
    }

    private void OnMouseExit()
    {
        Debug.Log("Player exit");
        if (isOutlined == true)
        {
            Color tempColor = playerHouse.houseRenderer.material.color;
            tempColor.a = .3f;
            playerHouse.houseRenderer.material.color = tempColor;

            gameObject.GetComponentInChildren<Outline>().enabled = false;
            isOutlined = false;
        }

        TooltipManager.Instance.clearHoveredData();
    }

    [PunRPC]
    public void RPC_animationFinished()
    {
        isSequenceRunning = false;
    }

    public void ignoreRaycast()
    {
        gameObject.layer = ReferenceManager.Instance.LayerIgnoreRaycast;
    }

}
