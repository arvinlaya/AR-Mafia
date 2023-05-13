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
    private bool isInitialized = false;
    int index;
    PhotonView PV;
    [SerializeField] Transform[] PlayerSpawn5;
    [SerializeField] Transform[] PlayerSpawn6;
    [SerializeField] Transform[] PlayerSpawn7;
    [SerializeField] Transform[] PlayerSpawn8;
    [SerializeField] public Transform middle;
    public Transform[] spawnPoints;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update

    public void SpawnPlayersAndHouses()
    {
        if (isInitialized == false)
        {
            Debug.Log("SPAWWWWWWWWWWWWWNING");
            isInitialized = true;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Directional Light"), Vector3.zero, Quaternion.identity);
            }
            index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PV.RPC(nameof(RPC_InstantiatePlayer), player, index);

                index++;
            }

            index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PV.RPC(nameof(RPC_setHouseAndPlayerParentWrapper), player, index, player.NickName);

                index++;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.startGenerateRoles();
            }
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
        spawnPoints = GetSpawnPoints(playerCount);

        Vector3 housePos = spawnPoints[index].position;
        GameObject house = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerHouse"), new Vector3(0, 0, 0), Quaternion.identity);
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0, 0, 0), Quaternion.identity);
        ReferenceManager.Instance.panelParent.SetActive(true);

    }

    [PunRPC]
    void RPC_setHouseAndPlayerParentWrapper(int index, string playerName)
    {
        PV.RPC(nameof(RPC_setHouseAndPlayerParent), RpcTarget.All, index, playerName);
    }

    [PunRPC]
    void RPC_setHouseAndPlayerParent(int index, string playerName)
    {
        Debug.Log("SET PLAYER OF: " + playerName);
        Player player = PlayerManager.getPlayerByName(playerName);

        HouseController houseController = PlayerManager.getPlayerHouseController(player);
        PlayerController playerController = PlayerManager.getPlayerController(player);

        houseController.transform.position = spawnPoints[index].transform.position;
        houseController.transform.SetParent(spawnPoints[index], true);
        houseController.transform.LookAt(SpawnManager.Instance.middle);
        houseController.transform.Rotate(0, 180, 0);

        playerController.transform.SetParent(spawnPoints[index], true);
        playerController.transform.position = spawnPoints[index].transform.position;
    }

}
