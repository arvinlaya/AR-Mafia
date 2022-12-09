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

    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text privateGameCode;
    //PART 2
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    //PART 2.5
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    //not yet used:
    // [SerializeField] GameObject startGameButton;

    private bool isPrivate = false;
    [SerializeField] TMP_InputField privateRoomNameInputField;
    string myString = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {

            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
            //MOVED, to OnJoinedLobby()
            //PhotonNetwork.NickName = "Player #" + Random.Range(0, 1000).ToString("0000");
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
        //Problem: nickname changes everytime player joins a room
        PhotonNetwork.NickName = "Player #" + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        //Pwede sa Private Room CODE:
        //if (string.IsNullOrEmpty(roomNameInputField.text))
        //{
        //    return;
        //}
        isPrivate = false;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "-R" + Random.Range(0, 1000).ToString("0000"));
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        //PUBLIC GAME
        if (!isPrivate)
        {

            MenuManager.Instance.OpenMenu("room");
            Debug.Log(PhotonNetwork.CurrentRoom.Name + "OnJoinedRoom() (Public)");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name + "'s Public Game";
        }
        //PRIVATE GAME
        else
        {
            MenuManager.Instance.OpenMenu("room private");
            Debug.Log("PRIVATE ROOM CODE: " + PhotonNetwork.CurrentRoom.Name + "\nOnJoinedRoom() (Private)");
            privateGameCode.text = PhotonNetwork.CurrentRoom.Name;
        }


        // For Player List
        Player[] players = PhotonNetwork.PlayerList;

        if (!isPrivate)
        {
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < players.Count(); i++)
            {
                //TODO: playerListCONTENT_Private
                Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }

        //        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        Debug.Log("Number of players in the room: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        Debug.Log("umalis..");
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    //	public override void OnMasterClientSwitched(Player newMasterClient)
    //	{
    //		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    //	}

    //	//public void StartGame()
    //	{
    //		PhotonNetwork.LoadLevel(1);
    //	}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //TODO: playerListCONTENT_Private
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    // PRIVATE ROOM Functions

    public void CreateRoomPrivate()
    {

        const string glyphs = "abcdefghijklmnopqrstuvwxyz"; //add the characters you want
        int charAmount = Random.Range(3, 3); //set those to the minimum and maximum length of your string
        for (int i = 0; i < charAmount; i++)
        {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }

        Debug.Log("Private Room Created");
        PhotonNetwork.CreateRoom(myString.ToUpper(),
            new RoomOptions { IsVisible = false, MaxPlayers = 5 }
            )
            ;
        MenuManager.Instance.OpenMenu("loading");
        isPrivate = true;
        Debug.Log("Needed: " + myString);
    }

    public void JoinRoomPrivate()
    {

        isPrivate = true;
        //Not Showing
        Debug.Log("INPUT: " + privateRoomNameInputField.text);
        Debug.Log("Needed: " + myString);

        PhotonNetwork.JoinRoom(privateRoomNameInputField.text.ToUpper());
        MenuManager.Instance.OpenMenu("loading");//but this one is firing?
        //try
        //{
        //    if (privateRoomNameInputField.text.Length! < 3)
        //    {
        //    }
        //}
        //finally
        //{
        //    Debug.Log("Error");
        //}
        Debug.Log(" JoinRoomPrivate()");

    }

}