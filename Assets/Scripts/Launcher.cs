using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    //[SerializeField] TMP_InputField roomNameInputField; //no need, autogenerated room name
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    //	void Awake()
    //	{
    //		Instance = this;
    //	}

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {

            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = "Player #" + Random.Range(0, 1000).ToString("0000");
        }
        else
        {
            Debug.Log("Already connected!");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        //if (string.IsNullOrEmpty(roomNameInputField.text))
        //{
        //    return;
        //}
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "-R" + Random.Range(0, 1000).ToString("0000"));
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + "'s Public Game";

        //Player[] players = PhotonNetwork.PlayerList;

        //        foreach (Transform child in playerListContent)
        //        {
        //            Destroy(child.gameObject);
        //        }

        //        for (int i = 0; i < players.Count(); i++)
        //        {
        //            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        //        }

        //        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    //	public override void OnMasterClientSwitched(Player newMasterClient)
    //	{
    //		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    //	}

    //	//public void StartGame()
    //	{
    //		PhotonNetwork.LoadLevel(1);
    //	}

    //	public void JoinRoom(RoomInfo info)
    //	{
    //		PhotonNetwork.JoinRoom(info.Name);
    //		MenuManager.Instance.OpenMenu("loading");
    //}

    //	public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //	{
    //		foreach(Transform trans in roomListContent)
    //		{
    //			Destroy(trans.gameObject);
    //		}
    //
    //		for(int i = 0; i < roomList.Count; i++)
    //		{
    //			if(roomList[i].RemovedFromList)
    //				continue;
    //			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
    //		}
    //	}
    //
    //	public override void OnPlayerEnteredRoom(Player newPlayer)
    //	{
    //		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    //	}
}