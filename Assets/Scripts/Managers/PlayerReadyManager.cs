//Sam, Lobby Scene, for Ready Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerReadyManager : MonoBehaviourPunCallbacks
{
    private static PlayerReadyManager instance;

    //CARDS/Buttons
    [SerializeField] GameObject waitingPlayerCardPublic;
    [SerializeField] GameObject waitingPlayerCardPrivate;

    [SerializeField] GameObject readyBtnCardPublic;
    [SerializeField] GameObject readyBtnCardPrivate;

    [SerializeField] GameObject startForMasterBtnCardPublic;
    [SerializeField] GameObject startForMasterBtnCardPrivate;


    public static PlayerReadyManager GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerReadyManager();
        }
        return instance;
    }

    private PlayerReadyManager() { }

    private int readyPlayersCount = 0;
    private int minimumPlayers = 5;

    public int ReadyPlayersCount
    {
        get { return readyPlayersCount; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        StartCoroutine(nameof(startWaitingForReady));
    }

    public void OnClickIncrementReadyPlayers()
    {
        photonView.RPC("IncrementReadyPlayersRPC", RpcTarget.MasterClient);

        //HIDE Ready bTN
        readyBtnCardPrivate.SetActive(false);
        readyBtnCardPublic.SetActive(false);

        waitingPlayerCardPublic.SetActive(false);
        waitingPlayerCardPrivate.SetActive(false);
    }

    [PunRPC]
    private void IncrementReadyPlayersRPC()
    {
        readyPlayersCount++;
        if(PhotonNetwork.IsMasterClient) Debug.LogError("Number of ready:" + readyPlayersCount);
    }

    private void SendReadySignal()
    {
        // Send a ready signal to the master client
        photonView.RPC("ReadySignalRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void DecrementReadyPlayers()
    {
        readyPlayersCount--;
    }

    [PunRPC]
    private void ReadySignalRPC()
    {
        // Call the increment ready players method on the PlayerReadyManager singleton
        PlayerReadyManager.instance.OnClickIncrementReadyPlayers();

        // Check if all players are ready
        if (PlayerReadyManager.instance.ReadyPlayersCount >= 5 &&
            PlayerReadyManager.instance.ReadyPlayersCount <= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            // All players are ready, Show StartButton to MasterClient
            photonView.RPC("ShowStartButtonToMaster", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void ShowStartButtonToMaster()
    {
        // Load the game scene here
    }

    public IEnumerator startWaitingForReady()
    {
        yield return new WaitUntil(() => readyPlayersCount+1 >= minimumPlayers);

        if (PhotonNetwork.IsMasterClient)
        {
            yield return StartCoroutine(UpdateUI());
        }
    }

    //TODO: update ui when all is ready
    private IEnumerator UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            waitingPlayerCardPrivate.SetActive(false);
            waitingPlayerCardPublic.SetActive(false);

            startForMasterBtnCardPublic.SetActive(true);
            startForMasterBtnCardPrivate.SetActive(true);

        }

        yield return new WaitForSeconds(1f);
    }

}

