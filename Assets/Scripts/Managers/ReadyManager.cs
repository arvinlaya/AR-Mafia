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
        if (state == true)
        {
            PV.RPC(nameof(RPC_setReady), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    public void RPC_setReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentReady += 1;

            if (currentReady == requiredReady)
            {
                setIsAllReady(true);
            }
        }
    }

    public void resetReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentReady = 0;
            setIsAllReady(false);
        }
    }

    public bool getIsAllReady()
    {
        return isAllReady;
    }

    private void setIsAllReady(bool state)
    {
        isAllReady = state;
    }

}
