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
    public event Action OnEvent;
    public static event Action<PhotonView> OnDoorEvent;
    // Start is called before the first frame update
    void Start()
    {
        ownerController = PlayerManager.getPlayerController(owner);
        transform.localPosition += offset;
        OnEvent = null;

        switch (GameManager.GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                OnEvent += OpenDoor;
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                OnEvent = null;
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
                OnEvent += AccuseVote;
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                OnEvent = null;
                break;

            case GameManager.GAME_PHASE.DAY_VOTE:
                OnEvent += Vote;
                transform.position = ownerController.transform.position;
                transform.localPosition += offset;
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
                    OnEvent?.Invoke();
                }
            }
        }

    }

    void OpenDoor()
    {
        if (GameManager.Instance.isDoorCooldown() == false)
        {
            OnDoorEvent?.Invoke(house.PV);
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
