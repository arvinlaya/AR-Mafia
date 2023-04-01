using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReadyManager : MonoBehaviour
{
    public static ReadyManager Instance;
    private int requiredReady;
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

    public void setRequiredReady(int requiredReady)
    {
        this.requiredReady = requiredReady;
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

            if (currentReady == requiredReady)
            {
                PV.RPC(nameof(RPC_setIsAllReady), RpcTarget.All, true);
            }
        }
    }

    public void resetReady()
    {
        currentReady = 0;
        isAllReady = false;
    }

    public bool getIsAllReady()
    {
        return isAllReady;
    }

    [PunRPC]
    private void RPC_setIsAllReady(bool state)
    {
        isAllReady = state;
    }

}
