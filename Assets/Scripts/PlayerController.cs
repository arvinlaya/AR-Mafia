using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private MeshFilter characterModel;
    private bool isSet;

    Player player;
    PhotonView PV;
    [SerializeField] LeftButton LeftButtonPrefab;
    [SerializeField] RightButton RightButtonPrefab;
    public bool buttonActive;


    void Awake()
    {
        isSet = false;
        player = PhotonNetwork.LocalPlayer;
        PV = GetComponent<PhotonView>();
        buttonActive = false;
    }

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
                        LeftButton leftButtonTemp;
                        RightButton rightButtonTemp;
                        leftButtonTemp = Instantiate(LeftButtonPrefab, hitPV.transform.position, Quaternion.identity);
                        rightButtonTemp = Instantiate(RightButtonPrefab, hitPV.transform.position, Quaternion.identity);

                        leftButtonTemp.house = hitPV.GetComponent<HouseController>();
                        leftButtonTemp.owner = hitPV.Owner;

                        rightButtonTemp.house = hitPV.GetComponent<HouseController>();
                        rightButtonTemp.owner = hitPV.Owner;

                    }
                }
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_OnSetRole", targetPlayer, changedProps["ROLE"], targetPlayer.NickName);
        }
    }

    [PunRPC]
    void RPC_OnSetRole(string role, string targetName)
    {
        object[] data = { (FindObjectsOfType<PlayerController>().SingleOrDefault(x => x.PV.Owner.NickName == targetName)).GetComponent<PhotonView>().ViewID };

        GameObject model = null;
        if (isSet == false)
        {
            isSet = true;
        }
        else
        {
            return;
        }

        switch (role)
        {
            case "VILLAGER":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Base"), transform.position, Quaternion.identity, 0, data);
                break;

            case "DOCTOR":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Doctor"), transform.position, Quaternion.identity, 0, data);
                break;

            case "MAFIA":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mafia"), transform.position, Quaternion.identity, 0, data);
                break;

            case "POLICE":
                model = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Police"), transform.position, Quaternion.identity, 0, data);
                break;

            default:
                Debug.Log("MESH ERROR");
                break;
        }



    }
}
