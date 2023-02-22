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

    private bool isPrivate = false;
    [SerializeField] TMP_InputField privateRoomNameInputField;
    [SerializeField] Transform playerListContentPrivate;
    private const int joinPrivateCodeLength = 3;

    //PlayerList

    //Max player
    private const int _maxPlayer = 3;

    //START GAME
    [SerializeField] TMP_Text waitingForPlayersText;
    [SerializeField] GameObject waitingPlayerCardPublic;
    [SerializeField] GameObject waitingPlayerCardPrivate;



    [SerializeField] TMP_Text hostNamePublic;
    [SerializeField] TMP_Text hostNamePrivate;

    [SerializeField] GameObject startGameButtonPublic;
    [SerializeField] GameObject startGameButtonPrivate;

    [SerializeField] TMP_Text privateGameNumberOfPlayers;
    [SerializeField] TMP_Text publicGameNumberOfPlayers;

    Hashtable hashRoomOwner = new Hashtable();

    [SerializeField] GameObject ignModal;
    [SerializeField] TMP_InputField ignInputField;
    [SerializeField] TMP_Text ignText;
    [SerializeField] GameObject iconIgn;

    [SerializeField] GameObject nameTooLongPrompt;
    [SerializeField] GameObject nameIsEmptyPrompt;


    [SerializeField] GameObject wasKickedPromt;

    private bool leftNotKicked = true;

    private bool gameStarted = false;

    private PhotonView PV;

    [PunRPC]
    void RPC_StartGame()
    {
        //NOTE: Wala ng ikot, "loadingMenu" na dinaanan
        StartCoroutine(waiter());
    }

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
        }
        else
        {
            Debug.Log("Already connected!");
        }

    }

    void Update()
    {
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        Debug.Log("nickname: " + PhotonNetwork.NickName);
        ignModal.gameObject.SetActive(PhotonNetwork.NickName == "");
        //PhotonNetwork.NickName = "P#" + Random.Range(0, 100).ToString("000");
    }

    public void setIGN()
    {
        if (ignInputField.text != "" && ignInputField.text.Length <= 12)
        {
            ignModal.gameObject.SetActive(false);
            PhotonNetwork.NickName = ignInputField.text.ToUpper();
            Debug.Log(PhotonNetwork.NickName);
            ignText.text = ignInputField.text.ToUpper();
            ignText.gameObject.SetActive(true);
            iconIgn.gameObject.SetActive(false);
        }

        else if (ignInputField.text.Length > 12)
        {
            Debug.Log("\nToo long");
            nameTooLongPrompt.gameObject.SetActive(true);
        }

        else if (ignInputField.text == "")
        {
            nameIsEmptyPrompt.gameObject.SetActive(true);
            Debug.Log("\nCannot be Empty String");
        }

        else
        {
            Debug.Log("Something went wrong.");
        }
    }

    public void CreateRoom()
    {
        isPrivate = false;
        PhotonNetwork.CreateRoom("R-" + Random.Range(0, 1000).ToString("0000"));
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        // if (PhotonNetwork.IsMasterClient)
        // {
        // hashRoomOwner.Add("RoomOwner", PhotonNetwork.NickName);
        // PhotonNetwork.CurrentRoom.SetCustomProperties(hashRoomOwner);
        // Debug.Log("setting Room Owner HASH\n" + (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomOwner"]);
        // }

        //PUBLIC GAME
        if (!isPrivate)
        {
            MenuManager.Instance.OpenMenu("room");
            Debug.Log(PhotonNetwork.CurrentRoom.Name + "OnJoinedRoom() (Public)");
            roomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            publicGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";

            if (PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
            {
                Debug.Log("agay dito yung pagka join ko");
                waitingForPlayersText.text = "GET READY";
            }

            string roomMasterName = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName;
            hostNamePublic.text = roomMasterName;

        }
        //PRIVATE GAME
        else
        {
            MenuManager.Instance.OpenMenu("room private");
            Debug.Log("PRIVATE ROOM CODE: " + PhotonNetwork.CurrentRoom.Name + "\nOnJoinedRoom() (Private)");
            privateGameHostName.text = PhotonNetwork.MasterClient.NickName;//Bianca, sa onjoinedroom dati...
            privateGameCode.text = PhotonNetwork.CurrentRoom.Name;
            privateGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";

            string roomMasterName = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName;
            hostNamePrivate.text = roomMasterName;
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
                Instantiate(PlayerListItemPrefab, playerListContentPrivate).GetComponent<PlayerListItem>().SetUp(players[i]);
            }
        }


        //startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        Debug.Log("Number of players in the room: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }

    //Show Start button only when reached max players
    private void IsMaxPlayer(bool isMax)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButtonPublic.SetActive(isMax);
            startGameButtonPrivate.SetActive(isMax);
            waitingPlayerCardPublic.SetActive(!isMax);
            waitingPlayerCardPrivate.SetActive(!isMax);
        }
    }

    public void LeaveRoom()
    {
        leftNotKicked = true;
        IsMaxPlayer(false);//not max player, someone left...
        PhotonNetwork.CurrentRoom.IsOpen = true;//has slot
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
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        privateGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";
        publicGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";
    }

    //ONLY called when list of rooms change, not specific rooms
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayer)
            startGameButtonPublic.SetActive(PhotonNetwork.IsMasterClient);
    }

    IEnumerator waiter()
    {
        //TODO: If Else Kung ilan yung Players, kung ilan yung number of players in the room
        //NOTE: Max is 8, minimum is 5... 5 or 6 or 7 or 8

        //NOTE: For final product, ganito yung lalagay:
        Debug.Log("Loading Pre-game Screen # : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        //MenuManager.Instance.OpenMenu("pre-game-"+PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        //... While testing, ganito muna... "laging sa 5 players..."
        MenuManager.Instance.OpenMenu("pre-game-5");


        //Wait for 5 seconds
        yield return new WaitForSeconds(5);
        PhotonNetwork.LoadLevel(1);//1 = build settings index, actual game...
    }

    public void StartGame()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_StartGame", RpcTarget.All);
        }
    }

    void OnStartGame()
    {

    }

    //sa MasterClient lang may trigger yung function na 'to
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!isPrivate)
        {
            publicGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }
        else
        {
            privateGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8";
            Instantiate(PlayerListItemPrefab, playerListContentPrivate).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }
        //Debug.Log("PLAYER COUNT: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + _maxPlayer);
        ////STARTING GAME
        bool isMax = PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayer;
        if (isMax)
        {
            IsMaxPlayer(isMax);
            // pag max na, hide room 
            PhotonNetwork.CurrentRoom.IsOpen = !isMax;
        }
    }

    // PRIVATE ROOM Functions

    public void CreateRoomPrivate()
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz"; //add the characters you want
        int charAmount = Random.Range(joinPrivateCodeLength, joinPrivateCodeLength); //set those to the minimum and maximum length of your string
        string stringToCreatePrivateRoom = "";
        for (int i = 0; i < charAmount; i++)
        {
            stringToCreatePrivateRoom += glyphs[Random.Range(0, glyphs.Length)];
        }

        PhotonNetwork.CreateRoom(stringToCreatePrivateRoom.ToUpper(),
            new RoomOptions { IsVisible = false, MaxPlayers = _maxPlayer, }
            )
            ;
        MenuManager.Instance.OpenMenu("loading");
        isPrivate = true;
    }

    public void JoinRoomPrivate()
    {
        isPrivate = true;
        PhotonNetwork.JoinRoom(privateRoomNameInputField.text.ToUpper());
        MenuManager.Instance.OpenMenu("loading");

    }

    [PunRPC]
    void RPC_meWasKicked()
    {
        wasKickedPromt.gameObject.SetActive(true);
    }

    public void KickPlayer(Player foreignPlayer)
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_meWasKicked", foreignPlayer);
        }

        if (PhotonNetwork.LocalPlayer == foreignPlayer)
        {
            leftNotKicked = false;
        }

        PhotonNetwork.CloseConnection(foreignPlayer);
        //not "max/set player", you kicked somone
        IsMaxPlayer(false);
        PhotonNetwork.CurrentRoom.IsOpen = true;//has slot
    }

}