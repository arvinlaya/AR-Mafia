using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LeftButton : MonoBehaviour
{
    Vector3 offset = new Vector3(-1f, 5, 0);
    PlayerController ownerController;
    public HouseController house { get; set; }
    public Player owner { get; set; }

    private Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        ownerController = PlayerManager.getPlayerController(owner);
        transform.localPosition += offset;
        GameManager.Instance.OnPhaseChange += ChangePhase;
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
    }

    void ChangePhase()
    {
        switch (GameManager.Instance.GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                // OnEvent += OpenDoor;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[0];
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                // OnEvent = null;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                // OnEvent += AccuseVote;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[4];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                // OnEvent = null;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_VOTE:
                // OnEvent += Vote;
                transform.position = ownerController.transform.position;
                transform.localPosition += offset;
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[6];
                break;
        }
    }

    void OnClick(GameManager.GAME_PHASE GAME_STATE)
    {
        switch (GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                StartCoroutine(nameof(OpenDoor));
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                // AccuseVote();
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                break;

            case GameManager.GAME_PHASE.DAY_VOTE:
                Vote();
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == gameObject.transform)
                {
                    StartCoroutine(nameof(clickFeedback));
                    OnClick(GameManager.Instance.GAME_STATE);
                }
            }
        }

    }

    IEnumerator OpenDoor()
    {

        if (GameManager.Instance.openDoorOnCooldown == false)
        {
            HouseController[] controllers = GameObject.FindObjectsOfType<HouseController>();
            foreach (HouseController controller in controllers)
            {
                house.DoorEvent(house.PV);
            }
            GameManager.Instance.setDoorCooldown(true);
        }

        yield return new WaitForSeconds(2f);

        PlayerController callerController = PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer);
        PlayerController ownerController = PlayerManager.getPlayerController(owner);
        callerController.enterHouseSequence(house.PV.ViewID, ownerController.PV.ViewID);
    }

    void Vote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            CustomPropertyWrapper.incrementProperty(owner, "INNOCENT_VOTE", 1);

            CustomPropertyWrapper.decrementProperty(PhotonNetwork.LocalPlayer, "VOTE_VALUE", 1);
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 1)
        {
            CustomPropertyWrapper.decrementProperty(owner, "GUILTY_VOTE", 1);

            CustomPropertyWrapper.incrementProperty(owner, "INNOCENT_VOTE", 1);

            CustomPropertyWrapper.setPropertyInt(PhotonNetwork.LocalPlayer, "VOTE_VALUE", -1);
        }
    }

    IEnumerator clickFeedback()
    {
        gameObject.transform.localScale += new Vector3(-.15f, -.15f, -.15f);

        yield return new WaitForSeconds(.1f);

        gameObject.transform.localScale += new Vector3(.15f, .15f, .15f);
    }
}
