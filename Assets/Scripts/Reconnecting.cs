using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Reconnecting : IConnectionCallbacks
{
    private LoadBalancingClient loadBalancingClient;
    private AppSettings appSettings;

    public Reconnecting(LoadBalancingClient loadBalancingClient, AppSettings appSettings)
    {
        this.loadBalancingClient = loadBalancingClient;
        this.appSettings = appSettings;
        this.loadBalancingClient.AddCallbackTarget(this);
    }

    ~Reconnecting()
    {
        this.loadBalancingClient.RemoveCallbackTarget(this);
    }

    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
    {
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
    }

    #region Unused Methods

    void IConnectionCallbacks.OnConnected()
    {
    }

    void IConnectionCallbacks.OnConnectedToMaster()
    {
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

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    #endregion
}
