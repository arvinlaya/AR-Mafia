using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Photon.Realtime;
using Photon.Pun;


public class Reconnecting :   MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    private LoadBalancingClient loadBalancingClient;
    private AppSettings appSettings;

    [SerializeField] public GameObject disconnectPrompt;

    public Reconnecting (LoadBalancingClient loadBalancingClient, AppSettings appSettings)
    {
        this.loadBalancingClient = loadBalancingClient;
        this.appSettings = appSettings;
        this.loadBalancingClient.AddCallbackTarget(this);
    }

    ~Reconnecting ()
    {
        this.loadBalancingClient.RemoveCallbackTarget(this);
    }

    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected, ScriptFromPhotonSample.cs");
        disconnectPrompt.SetActive(true);

        if (this.CanRecoverFromDisconnect(cause))
        {
            this.Recover();
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    private void Recover()
    {
        Debug.Log("SAM-RECO");
        if (!loadBalancingClient.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!loadBalancingClient.ReconnectToMaster())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!loadBalancingClient.ConnectUsingSettings(appSettings))
                {
                    Debug.LogError("ConnectUsingSettings failed");
                }
            }
        }
        else
        {
            Debug.Log("Gumana,,, SAM RECO == loadbalancing client = true");
        }
    }

    #region Unused Methods

    void IConnectionCallbacks.OnConnected()
    {
        Debug.Log("Suxx, OnConnected");
        disconnectPrompt.SetActive(false);
    }

    void IConnectionCallbacks.OnConnectedToMaster()
    {
        Debug.Log("Suxx, OnConnectedToMaster");
    }

    void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    #endregion

    //SAM
	public void OnClickReconnect()
	{
        Debug.Log("You clicked Reconnect... wait");
        Debug.Log(loadBalancingClient.ReconnectAndRejoin()+"If false, attemp failed");
	}

    //public override void OnConnected()
    //{
    //    Debug.Log("connected again..");
    //    disconnectPrompt.SetActive(false);
    //}
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("Rejoined room");
    //    Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

    //}
}
