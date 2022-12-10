using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text privateGameCode;
    [SerializeField] TMP_Text privateGameHostName;
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
    string stringToCreatePrivateRoom = "";
    [SerializeField] Transform playerListContentPrivate;

    //PlayerList


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
            PhotonNetwork.EnableCloseConnection = true;
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
        PhotonNetwork.NickName = "P#" + Random.Range(0, 100).ToString("000");
    }

    public void CreateRoom()
    {
        //Pwede sa Private Room CODE:
        //if (string.IsNullOrEmpty(roomNameInputField.text))
        //{
        //    return;
        //}
        isPrivate = false;
        PhotonNetwork.CreateRoom("R-" + Random.Range(0, 1000).ToString("0000"));
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        //PUBLIC GAME
        if (!isPrivate)
        {

            MenuManager.Instance.OpenMenu("room");
            Debug.Log(PhotonNetwork.CurrentRoom.Name + "OnJoinedRoom() (Public)");
            roomNameText.text = PhotonNetwork.MasterClient.NickName + "'s Public Game";
        }
        //PRIVATE GAME
        else
        {
            MenuManager.Instance.OpenMenu("room private");
            Debug.Log("PRIVATE ROOM CODE: " + PhotonNetwork.CurrentRoom.Name + "\nOnJoinedRoom() (Private)");
            privateGameHostName.text = PhotonNetwork.MasterClient.NickName;//Bianca, sa onjoinedroom dati...
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
                //TODO: playerListCONTENT_Public
                Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }
        else
        {

            foreach (Transform child in playerListContentPrivate)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < players.Count(); i++)
            {
                //TODO: playerListCONTENT_Private
                Instantiate(PlayerListItemPrefab, playerListContentPrivate).GetComponent<PlayerListItem>().SetUp(players[i]);
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
        if (!isPrivate)
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        else
            Instantiate(PlayerListItemPrefab, playerListContentPrivate).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    // PRIVATE ROOM Functions

    public void CreateRoomPrivate()
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz"; //add the characters you want
        int charAmount = Random.Range(3, 3); //set those to the minimum and maximum length of your string
        for (int i = 0; i < charAmount; i++)
        {
            stringToCreatePrivateRoom += glyphs[Random.Range(0, glyphs.Length)];
        }

        Debug.Log("Private Room Created");
        PhotonNetwork.CreateRoom(stringToCreatePrivateRoom.ToUpper(),
            new RoomOptions { IsVisible = false, MaxPlayers = 5, }
            )
            ;
        MenuManager.Instance.OpenMenu("loading");
        isPrivate = true;
    }

    public void JoinRoomPrivate()
    {

        isPrivate = true;
        //Not Showing
        Debug.Log("INPUT: " + privateRoomNameInputField.text);
        Debug.Log("Needed: " + stringToCreatePrivateRoom);

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

    public void KickPlayer(Player foreignPlayer)
    {
        PhotonNetwork.CloseConnection(foreignPlayer);
        Debug.Log("### Kicking player.." + foreignPlayer.NickName);
        Debug.Log("" + foreignPlayer.NickName);
        Debug.Log("Number of players in the room: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        //MenuManager.Instance.OpenMenu("loading");
    }
}