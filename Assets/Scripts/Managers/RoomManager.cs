using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    PhotonView PV;
    public bool isSet;
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
    }

    public override void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient && Instance.isSet == false)
        {

            Instance.PV = GetComponent<PhotonView>();
            if (scene.buildIndex == 1)
            {
                Instance.isSet = true;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    Instance.PV.RPC(nameof(RPC_instantantiatePlayerManager), player);
                }
            }
        }

    }

    [PunRPC]
    void RPC_instantantiatePlayerManager()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }
}
