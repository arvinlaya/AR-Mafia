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
        switch (GameManager.GAME_STATE)
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
                OpenDoor();
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                AccuseVote();
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
                    OnClick(GameManager.GAME_STATE);
                }
            }
        }

    }

    void OpenDoor()
    {
        Debug.Log("Already cooldown");
        if (GameManager.Instance.isDoorCooldown() == false)
        {
            Debug.Log("not cooldown");
            HouseController[] controllers = GameObject.FindObjectsOfType<HouseController>();
            foreach (HouseController controller in controllers)
            {
                house.DoorEvent(house.PV);
            }
            GameManager.Instance.setDoorCooldown();
        }
    }

    void AccuseVote()
    {
        Debug.Log("TEST");
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            int voteCount = 1 + (int)owner.CustomProperties["ACCUSE_VOTE_COUNT"];
            owner.SetCustomProperties(new Hashtable() { { "ACCUSE_VOTE_COUNT", voteCount } });

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTED", owner.NickName } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTE_VALUE", 1 } });
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["VOTED"] == player.NickName)
                {
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTED", owner.NickName } });

                    int voteCount = (int)player.CustomProperties["ACCUSE_VOTE_COUNT"] - 1;
                    player.SetCustomProperties(new Hashtable() { { "ACCUSE_VOTE_COUNT", voteCount } });


                    voteCount = 1 + (int)owner.CustomProperties["ACCUSE_VOTE_COUNT"];
                    owner.SetCustomProperties(new Hashtable() { { "ACCUSE_VOTE_COUNT", voteCount } });

                }
            }
        }
    }

    void Vote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            int voteCount = 1 + (int)owner.CustomProperties["INNOCENT_VOTE"];
            owner.SetCustomProperties(new Hashtable() { { "INNOCENT_VOTE", voteCount } });

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTE_VALUE", -1 } });

        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 1)
        {
            int voteCount = (int)owner.CustomProperties["GUILTY_VOTE"] - 1;
            owner.SetCustomProperties(new Hashtable() { { "GUILTY_VOTE", voteCount } });

            voteCount = 1 + (int)owner.CustomProperties["INNOCENT_VOTE"];
            owner.SetCustomProperties(new Hashtable() { { "INNOCENT_VOTE", voteCount } });

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTE_VALUE", -1 } });
        }
    }

    IEnumerator clickFeedback()
    {
        gameObject.transform.localScale += new Vector3(-.15f, -.15f, -.15f);

        yield return new WaitForSeconds(.1f);

        gameObject.transform.localScale += new Vector3(.15f, .15f, .15f);
    }
}
