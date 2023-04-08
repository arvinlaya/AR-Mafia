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
        Debug.Log("ENABLED ONSCENELOADED IN ROOMMANAGER");
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("DISABLED ONSCENELOADED IN ROOMMANAGER");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Instance.PV = GetComponent<PhotonView>();
            if (scene.buildIndex == 1)
            {
                Debug.Log("CALLED ONSCENELOADED BY MASTERCLIENT");
                Instance.PV.RPC(nameof(RPC_instantantiatePlayerManager), RpcTarget.All);
            }
        }

    }

    [PunRPC]
    void RPC_instantantiatePlayerManager()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }
}
