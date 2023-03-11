using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class SpawnManager : MonoBehaviour
{
    int playerCount = 5;
    public static SpawnManager Instance;
    int index;

    public bool isSet;
    PhotonView PV;
    [SerializeField] Transform[] PlayerSpawn5;
    [SerializeField] Transform[] PlayerSpawn6;
    [SerializeField] Transform[] PlayerSpawn7;
    [SerializeField] Transform[] PlayerSpawn8;

    public Transform[] spawnPoints;

    public IDictionary<Player, Vector3> playerSpawn;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Instance.isSet = false;
        playerSpawn = new Dictionary<Player, Vector3>();
    }
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GetSpawnPoints(playerCount);
    }

    public void SpawnPlayersAndHouses()
    {
        if (Instance.isSet == false)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Directional Light"), Vector3.zero, Quaternion.identity);
            }
            index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PV.RPC(nameof(RPC_InstantiatePlayer), player, index);
                PV.RPC(nameof(RPC_InstantiatePlayerSpawnPoints), RpcTarget.All, player.NickName, index);

                index++;
            }
            Instance.isSet = true;
        }


    }

    Transform[] GetSpawnPoints(int playerCount)
    {
        if (playerCount == 5)
        {
            return PlayerSpawn5;
        }
        else if (playerCount == 6)
        {
            return PlayerSpawn6;
        }
        else if (playerCount == 7)
        {
            return PlayerSpawn7;
        }
        else if (playerCount == 8)
        {
            return PlayerSpawn8;
        }
        return null;
    }

    [PunRPC]
    void RPC_InstantiatePlayer(int index)
    {
        Vector3 housePos = spawnPoints[index].position;
        GameObject house = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerHouse"), housePos, Quaternion.identity);
        Vector3 playerPos = house.GetComponent<HouseController>().ownerLocation.position;

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), playerPos, Quaternion.identity);
    }

    [PunRPC]
    void RPC_InstantiatePlayerSpawnPoints(string playerNickname, int index)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerNickname)
            {
                playerSpawn.Add(player, spawnPoints[index].position);
            }
        }
    }
}
