using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class LeftButton : MonoBehaviour
{
    Vector3 offset = new Vector3(0, 2, 0);
    public HouseController house { get; set; }
    public Player owner { get; set; }
    public event Action OnEvent;
    // Start is called before the first frame update
    void Start()
    {
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
                    OnEvent?.Invoke();
                }
            }
        }
    }

    void OpenDoor()
    {
        Debug.Log("DOOR OPENED");
    }

    void AccuseVote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            int voteCount = 1 + (int)owner.CustomProperties["ACCUSE_VOTE_COUNT"];
            owner.CustomProperties["ACCUSE_VOTE_COUNT"] = voteCount;
            PhotonNetwork.LocalPlayer.CustomProperties["VOTED"] = owner.NickName;
            PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] = 1;
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["VOTED"] == player.NickName)
                {
                    PhotonNetwork.LocalPlayer.CustomProperties["VOTED"] = owner.NickName;

                    int voteCount = (int)player.CustomProperties["ACCUSE_VOTE_COUNT"] - 1;
                    player.CustomProperties["ACCUSE_VOTE_COUNT"] = voteCount;

                    voteCount = 1 + (int)owner.CustomProperties["ACCUSE_VOTE_COUNT"];
                    owner.CustomProperties["ACCUSE_VOTE_COUNT"] = voteCount;
                }
            }
        }
    }

    void Vote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            int voteCount = 1 + (int)owner.CustomProperties["INNOCENT_VOTE"];
            owner.CustomProperties["INNOCENT_VOTE"] = voteCount;
            PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] = -1;
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 1)
        {
            int voteCount = (int)owner.CustomProperties["GUILTY_VOTE"] - 1;
            owner.CustomProperties["GUILTY_VOTE"] = voteCount;

            voteCount = 1 + (int)owner.CustomProperties["INNOCENT_VOTE"];
            owner.CustomProperties["INNOCENT_VOTE"] = voteCount;
            PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] = -1;
        }
    }
}
