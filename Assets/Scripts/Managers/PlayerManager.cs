using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class PlayerManager : MonoBehaviour
{
    private bool started;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        started = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PhotonNetwork.IsMasterClient && PV.IsMine && !started)
            {
                started = true;
                Debug.Log("SPAWN PLAYERS AND HOUSES");
                SpawnManager.Instance.SpawnPlayersAndHouses();
            }
        }
    }

    public static PlayerController getPlayerController(Player player)
    {
        foreach (PlayerController controller in GameObject.FindObjectsOfType<PlayerController>())
        {
            if (player.NickName == controller.PV.Owner.NickName)
            // if (player == controller.PV.Owner)
            {
                return controller;
            }
        }
        return null;
    }

    public static Player getPlayerByName(string playerName)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                return player;
            }
        }
        return null;
    }

    public static HouseController getPlayerHouseController(Player player)
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            if (player == controller.PV.Owner)
            {
                return controller;
            }
        }
        return null;
    }

}
