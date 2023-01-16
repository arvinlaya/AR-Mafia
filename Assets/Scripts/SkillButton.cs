using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class SkillButton : MonoBehaviour
{
    Vector3 offset = new Vector3(1.2f, 2, 0);
    public Player target { get; set; }



    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition += offset;
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
                    switch (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
                    {
                        case "VILLAGER":
                            new Villager().skill(target);
                            break;

                        case "DOCTOR":
                            new Doctor().skill(target);
                            break;

                        case "MAFIA":
                            new Mafia().skill(target);
                            break;

                        case "POLICE":
                            new Police().skill(target);
                            break;

                        default:
                            Debug.Log("ROLE NOT FOUND...");
                            break;
                    }
                }
            }
        }

    }
}
