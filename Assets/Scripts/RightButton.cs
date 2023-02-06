using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RightButton : MonoBehaviour
{
    Vector3 offset = new Vector3(1f, 5, 0);
    PlayerController ownerController;

    public HouseController house { get; set; }

    public Player owner { get; set; }
    public event Action OnEvent;

    // Start is called before the first frame update
    void Start()
    {
        ownerController = PlayerManager.getPlayerController(owner);
        transform.localPosition += offset;
        OnEvent = null;

        switch (GameManager.GAME_STATE)
        {
            case GameManager.GAME_PHASE.NIGHT:
                OnEvent += Skill;
                break;

            case GameManager.GAME_PHASE.DAY_DISCUSSION:
                OnEvent = null;
                break;

            case GameManager.GAME_PHASE.DAY_ACCUSE:
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

    void Skill()
    {
        switch (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
        {
            case "VILLAGER":
                new Villager().skill(owner);
                break;

            case "DOCTOR":
                new Doctor().skill(owner);
                break;

            case "MAFIA":
                new Mafia().skill(owner);
                break;

            case "DETECTIVE":
                new Detective().skill(owner);
                break;

            default:
                Debug.Log("ROLE NOT FOUND...");
                break;
        }
    }

    void Vote()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == 0)
        {
            int voteCount = 1 + (int)owner.CustomProperties["GUILTY_VOTE"];
            owner.SetCustomProperties(new Hashtable() { { "GUILTY_VOTE", voteCount } });

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "VOTE_VALUE", 1 } });

        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["VOTE_VALUE"] == -1)
        {
            int voteCount = (int)owner.CustomProperties["INNOCENT_VOTE"] - 1;
            owner.SetCustomProperties(new Hashtable() { { "INNOCENT_VOTE", voteCount } });

            voteCount = 1 + (int)owner.CustomProperties["GUILTY_VOTE"];
            owner.SetCustomProperties(new Hashtable() { { "GUILTY_VOTE", voteCount } });

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
