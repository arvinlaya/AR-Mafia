using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;

public class RightButton : MonoBehaviour
{
    Vector3 offset = new Vector3(1.2f, 2, 0);

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

            case "POLICE":
                new Police().skill(owner);
                break;

            default:
                Debug.Log("ROLE NOT FOUND...");
                break;
        }
    }
}
