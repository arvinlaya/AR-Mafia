using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReadyManager : MonoBehaviour
{
    public static ReadyManager Instance;
    private int currentReady;
    private bool isAllReady;
    public PhotonView PV;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentReady = 0;
        isAllReady = false;
    }

    public void setReady(bool state)
    {
        PV.RPC(nameof(RPC_setReady), RpcTarget.MasterClient, state);
    }

    [PunRPC]
    public void RPC_setReady(bool state)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentReady += state ? 1 : -1;

            Debug.Log("currentReady: " + currentReady + " requiredReady: " + GameManager.Instance.getAliveList().Count);

            if (currentReady >= GameManager.Instance.getAliveList().Count)
            {
                PV.RPC(nameof(RPC_setIsAllReady), RpcTarget.All, true);
            }
        }
    }

    public void resetReady()
    {
        Debug.Log("resetReady");
        currentReady = 0;
        isAllReady = false;
    }

    public bool getIsAllReady()
    {
        if (isAllReady)
        {
            return true;
            resetReady();
        }
        else
        {
            return false;
        }
    }

    [PunRPC]
    private void RPC_setIsAllReady(bool state)
    {
        isAllReady = state;
    }

}
