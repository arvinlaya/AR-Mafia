using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;


public class HouseController : MonoBehaviour
{

    PhotonView PV;
    [SerializeField] OpenDoorButton openDoorButtonPrefab;
    [SerializeField] SkillButton skillButtonPrefab;
    public bool buttonActive;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        buttonActive = false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
                        OpenDoorButton doorTemp;
                        SkillButton skillTemp;
                        doorTemp = Instantiate(openDoorButtonPrefab, hitPV.transform.position, Quaternion.identity);
                        skillTemp = Instantiate(skillButtonPrefab, hitPV.transform.position, Quaternion.identity);

                        doorTemp.house = hitPV.GetComponent<HouseController>();
                        skillTemp.target = hitPV.Owner;
                    }
                }
            }
        }

    }
}
