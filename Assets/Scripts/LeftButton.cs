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
    public Vector3 offset = new Vector3(-.2f, .5f, 0);
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

        gameObject.SetActive(false);
    }

    void ChangePhase()
    {
        switch (GameManager.Instance.GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[0];
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[4];
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                renderer.sharedMaterial = ReferenceManager.Instance.ButtonMaterials[7];
                break;
        }
    }

    void OnClick(GameManager.GAME_PHASE GAME_STATE)
    {
        switch (GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                StartCoroutine(methodName: nameof(OpenDoor));
                break;

                // case GameManager.GAME_PHASE.DAY_DISCUSSION:
                //     break;

                // case GameManager.GAME_PHASE.DAY_ACCUSE:
                //     // AccuseVote();
                //     break;

                // case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                //     break;

                // case GameManager.GAME_PHASE.DAY_VOTE:
                //     Vote();
                //     break;
        }
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     RaycastHit hit;
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         if (hit.transform == gameObject.transform)
        //         {
        //             StartCoroutine(nameof(clickFeedback));
        //             OnClick(GameManager.Instance.GAME_STATE);
        //         }
        //     }
        // }

        GameManager.Instance.rotateToCamera(gameObject.transform, ReferenceManager.Instance.camera.transform, 10f, 0, 0);
    }

    private void OnMouseDown()
    {
        StartCoroutine(nameof(clickFeedback));
        OnClick(GameManager.Instance.GAME_STATE);
    }

    IEnumerator OpenDoor()
    {
        if (CooldownManager.Instance.getIsDoorCooldown() == false)
        {
            HouseController[] controllers = GameObject.FindObjectsOfType<HouseController>();
            foreach (HouseController controller in controllers)
            {
                controller.startUnfadeHouse();
                controller.closeDoor();
            }
            house.startInsideHouse();
            StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DOOR_OPEN_CLOSE, 0f));

            foreach (HouseController controller in controllers)
            {
                house.DoorEvent(house.PV);
                SoundManager.Instance.playGameClip(SoundManager.DOOR_OPEN_CLOSE, 0);
            }
            CooldownManager.Instance.setDoorCooldown(true);
            CooldownManager.Instance.setDoorCastTime(ReferenceManager.Instance.time);

            LogManager.Instance.openDoorAction(PhotonNetwork.LocalPlayer.NickName, house.PV.Owner.NickName);

            yield return new WaitForSeconds(2f);

            PlayerController callerController = PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer);
            PlayerController ownerController = PlayerManager.getPlayerController(owner);

            callerController.enterHouseSequence(house.PV.ViewID, ownerController.PV.ViewID);

            PlayerManager.getPlayerController(owner).isInsideOf = house.PV.Owner;
        }
        else
        {
            LogManager.Instance.openDoorCooldown(CooldownManager.Instance.getDoorCooldownRemaining(ReferenceManager.Instance.time));
        }

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
