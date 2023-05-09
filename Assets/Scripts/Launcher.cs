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
    [SerializeField] TMP_Text roomNameText_private;
    [SerializeField] TMP_Text privateGameCode;
    [SerializeField] TMP_Text privateGameHostName;
    //PART 2
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform roomListContent_Private;
    //PART 2.5
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;

    private bool isPrivate = false;
    //[SerializeField] TMP_InputField privateRoomNameInputField;
    [SerializeField] Transform playerListContentPrivate;
    private const int joinPrivateCodeLength = 5;

    //PlayerList

    //Max player
    //Mali yung description, ito yung "# of p+ Launcher.Instance._layers before you can START the game from the Room
    // Original = 8,5
    private const int maxPlayer = 8;
    //Ito yung kailangan na players before mag start, at least ganito karami: 
    private const int minimumPlayer = 3;
    //START GAME

    //change to "Host can start" when min is met
    [SerializeField] TMP_Text waitingForPlayersTextPublic;
    [SerializeField] TMP_Text waitingForPlayersTextPrivate;
    [SerializeField] GameObject waitingPlayerCardPublic;
    [SerializeField] GameObject waitingPlayerCardPrivate;



    [SerializeField] TMP_Text hostNamePublic;
    [SerializeField] TMP_Text hostNamePrivate;

    [SerializeField] GameObject startGameButtonPublic;
    [SerializeField] GameObject startGameButtonPrivate;
    [SerializeField] TMP_Text startTextPublic;
    [SerializeField] TMP_Text startTextPrivate;

    [SerializeField] TMP_Text privateGameNumberOfPlayers;
    [SerializeField] TMP_Text publicGameNumberOfPlayers;

    Hashtable hashRoomOwner = new Hashtable();

    //IGN
    [SerializeField] GameObject ignModal;
    [SerializeField] TMP_InputField ignInputField;
    [SerializeField] TMP_InputField ignInputField_notModal;
    [SerializeField] GameObject iconIgn;

    [SerializeField] GameObject nameTooLongPrompt;
    [SerializeField] GameObject nameIsEmptyPrompt;


    [SerializeField] GameObject wasKickedPromt;

    private bool leftNotKicked = true;

    private bool gameStarted = false;

    private PhotonView PV;

    // this = player in room count
    // if readty is press, --
    // if 1 or less, show start bnutton
    private int countOfReadyPlayers = 0;

    private bool allIsReady = false;

    //RECONNECTION
    //ROOM OPTIONS
    RoomOptions roomOptions = new RoomOptions();


    // can be use to improve code in the future

    //[PunRPC]
    //void RPC_MinimumPlayerReached_Public()
    //{
    //    //e.g 2/2
    //    //change waiting text "Host can start"
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //    waitingPlayerCardPublic.SetActive(false);
    //    startGameButtonPublic.SetActive(true);
    //    }
    //    else { 
    //    waitingForPlayersTextPublic.text = "Host can start";
    //    }
    //}

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

    public int getMaxPlayer()
    {
        return maxPlayer;
    }

    public int getMinPlayer()
    {
        return minimumPlayer;
    }

    void Update()
    {
            if (allIsReady && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("ALL IS READY? " + allIsReady);
                waitingPlayerCardPublic.SetActive(false);
                waitingPlayerCardPrivate.SetActive(false);

                startGameButtonPublic.SetActive(true);
                startGameButtonPrivate.SetActive(true);
            }
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

        //PLAYERPREF
        if (PlayerPrefs.HasKey("Nickname"))
        {
            string nickname = PhotonNetwork.NickName;
            ignModal.gameObject.SetActive(false);
            iconIgn.gameObject.SetActive(false);
            nickname = PlayerPrefs.GetString("Nickname"); //GET the saved nickname
            ignInputField_notModal.text = nickname;
            //PhotonNetwork.NickName = nickname;
            PhotonNetwork.NickName = nickname + (new System.Random().Next(1, 100));
        }
        else
        {
            ignModal.gameObject.SetActive(true);
        }

        Debug.Log("nickname: " + PhotonNetwork.NickName);

    }
    public void OnClickClearPlayerPref()
    {
        Debug.LogError("Deleting saves");
        PlayerPrefs.DeleteAll();
    }

    public void setIGN()
    {

        if (ignInputField.text != "" && ignInputField.text.Length <= 12)
        {
            ignModal.gameObject.SetActive(false);

            string nickname = ignInputField.text;

            //show in the middle ign display after setting
            ignInputField_notModal.text = nickname;

            iconIgn.gameObject.SetActive(false);

            Debug.LogError("Saving this nickname: " + nickname);
            PlayerPrefs.SetString("Nickname", nickname); // save the nickname to PlayerPrefs
            PhotonNetwork.NickName = nickname;

        }

        else if (ignInputField.text.Length > 12)
        {
            Debug.Log("\nToo long: " + ignInputField.text);
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

    public void OnChangeResetIGN()
    {
        string updatedName = ignInputField_notModal.text;
        Debug.LogError("NEW NAME: " + ignInputField.text);
        if (updatedName != "" && updatedName.Length <= 12)
        {
            ignModal.gameObject.SetActive(false);

            string nickname = updatedName;

            //show in the middle ign display after setting
            updatedName = nickname;

            iconIgn.gameObject.SetActive(false);

            Debug.LogError("Saving this nickname: " + nickname);
            PlayerPrefs.SetString("Nickname", nickname); // save the nickname to PlayerPrefs
            PhotonNetwork.NickName = nickname;

        }

        else if (updatedName.Length > 12)
        {
            Debug.Log("\nToo long: " + updatedName);
            nameTooLongPrompt.gameObject.SetActive(true);
        }

        else if (updatedName == "")
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
        roomOptions.PlayerTtl = 5000; // 5 secs
        roomOptions.EmptyRoomTtl = 1; // 1ms
        roomOptions.MaxPlayers = maxPlayer;

        // Set custom room properties
        Hashtable props = new Hashtable();
        props.Add("NumReadyPlayers", 0);
        roomOptions.CustomRoomProperties = props;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "NumReadyPlayers" };

        isPrivate = false;
        PhotonNetwork.CreateRoom("R-" + Random.Range(0, 1000).ToString("0000"), roomOptions: roomOptions);

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
        SpawnPlayersInList();
    }

    private void SpawnPlayersInList()
    {

        //PUBLIC GAME
        if (!isPrivate)
        {
            MenuManager.Instance.OpenMenu("room");
            Debug.Log(PhotonNetwork.CurrentRoom.Name + "OnJoinedRoom() (Public)");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            publicGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer;

            string roomMasterName = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName;
            hostNamePublic.text = roomMasterName;

        }
        //PRIVATE GAME
        else
        {
            MenuManager.Instance.OpenMenu("room private");
            Debug.Log("PRIVATE ROOM CODE: " + PhotonNetwork.CurrentRoom.Name + "\nOnJoinedRoom() (Private)");
            privateGameHostName.text = PhotonNetwork.MasterClient.NickName;//Bianca, sa onjoinedroom dati...
            roomNameText_private.text = PhotonNetwork.CurrentRoom.Name;
            //TODO Room code Pho
            privateGameCode.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["password"];
            privateGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer;

            string roomMasterName = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName;
            hostNamePrivate.text = roomMasterName;
        }

        if (PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient && PhotonNetwork.CurrentRoom.PlayerCount == minimumPlayer)
        {
            waitingForPlayersTextPublic.text = "GET READY";
            waitingForPlayersTextPrivate.text = "GET READY";
        }
        else if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient && PhotonNetwork.CurrentRoom.PlayerCount == minimumPlayer)
        {
            waitingForPlayersTextPublic.text = "Waiting to Accept";
            waitingForPlayersTextPrivate.text = "Waiting to Accept";
        }
        else
        {
            waitingForPlayersTextPublic.text = "Need at least " + minimumPlayer + " player";
            waitingForPlayersTextPrivate.text = "Need at least " + minimumPlayer + " player";
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
                Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i], i + 1);
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
                Instantiate(PlayerListItemPrefab, playerListContentPrivate).GetComponent<PlayerListItem>().SetUp(players[i], i + 1);
            }
        }

        int currentNoPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        //for ready check
        //need to --,to start
        countOfReadyPlayers = currentNoPlayers;

        // x is >= 5
        if (currentNoPlayers >= minimumPlayer)
        {
            bool minimumMet = currentNoPlayers >= minimumPlayer;

            if (allIsReady && PhotonNetwork.IsMasterClient)
            {
                waitingPlayerCardPublic.SetActive(false);
                waitingPlayerCardPrivate.SetActive(false);

                startGameButtonPublic.SetActive(true);
                startGameButtonPrivate.SetActive(true);
            }
            else if (minimumMet)
            {
                // + Get READY IMPLEMENT
                waitingPlayerCardPublic.SetActive(true);
                waitingPlayerCardPrivate.SetActive(true);
            }
            else
            {
                waitingPlayerCardPublic.SetActive(true);
                waitingPlayerCardPrivate.SetActive(true);
                startGameButtonPublic.SetActive(false);
                startGameButtonPrivate.SetActive(false);
            }
        }

        Debug.Log("Number of players in the room: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    //Show Start button only when reached max players
    private void IsMaxPlayer(bool isMax)
    {
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        leftNotKicked = true;
        //IsMaxPlayer(false);//not max player, someone left...
        PhotonNetwork.CurrentRoom.IsOpen = true;//has slot
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom(false);
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
        privateGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer;
        publicGameNumberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer;
        SpawnPlayersInList();
        IsMaxPlayer(false);

    }

    IEnumerator RefreshRoomList()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            //PhotonNetwork.GetRoomList();
        }
    }

    //ONLY called when list of rooms change, not specific rooms
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        StartCoroutine(RefreshRoomList());

        Debug.LogError("New Room Created");

        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        foreach (Transform trans in roomListContent_Private)
        {
            Destroy(trans.gameObject);
        }

        //StartCoroutine(CheckPrivateRooms(roomList));


        if (roomList.Count > 0)
        {
            for (int i = 0; i < roomList.Count; i++)
            {

                RoomInfo currentRoom = roomList[i];

                if (currentRoom.CustomProperties.ContainsKey("isPrivate"))
                    Debug.Log("Found a private currentRoom: " + currentRoom.Name + " (password: " + (string)currentRoom.CustomProperties["password"] + ")");


                if (currentRoom.RemovedFromList)
                    continue; // ignore/don't display

                else if (currentRoom.CustomProperties.ContainsKey("isPrivate"))//is Private, different component
                {
                    Instantiate(roomListItemPrefab, roomListContent_Private).GetComponent<RoomListItem>().SetUp(currentRoom);
                }
                else
                    Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(currentRoom);

            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayer)
            startGameButtonPublic.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);//1 = build settings index, actual game...
        }


        PhotonNetwork.CurrentRoom.IsVisible = false;
        //PhotonNetwork.GetRoomList();

    }

    void OnStartGame()
    {

    }

    //sa MasterClient lang may trigger yung function na 'to
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ////STARTING GAME
        bool isMax = PhotonNetwork.CurrentRoom.PlayerCount == maxPlayer;
        if (isMax)
        {
            IsMaxPlayer(isMax);
            // pag max na, hide room 
            PhotonNetwork.CurrentRoom.IsOpen = !isMax;
        }
        else
        {
            IsMaxPlayer(false);
            // pag di pa max na, show room 
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        SpawnPlayersInList();
    }

    // PRIVATE ROOM Functions

    public void CreateRoomPrivate()
    {
        //Creating the Code for the Room
        const string glyphs = "abcdefghijklmnopqrstuvwxyz"; //add the characters you want
        int charAmount = Random.Range(joinPrivateCodeLength, joinPrivateCodeLength); //set those to the minimum and maximum length of your string
        string password = "";

        for (int i = 0; i < charAmount; i++)
        {
            password += glyphs[Random.Range(0, glyphs.Length)];
        }

        string roomName = "PR-" + Random.Range(0, 1000).ToString("0000");
        Debug.Log("CREATING PRIVATE ROOM:" + roomName + ":" + password);

        // Create a new room with custom properties
        Hashtable customRoomProperties = new Hashtable();
        RoomOptions roomOptions = new RoomOptions();
        customRoomProperties.Add("isPrivate", true);
        customRoomProperties.Add("password", password);
        customRoomProperties.Add("NumReadyPlayers", 0);
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "isPrivate", "password", "NumReadyPlayers" };
        roomOptions.MaxPlayers = 8;

        roomOptions.PlayerTtl = 5000; // 5 secs
        roomOptions.EmptyRoomTtl = 1; // 1ms

        PhotonNetwork.CreateRoom(roomName, roomOptions);

        MenuManager.Instance.OpenMenu("loading");
        isPrivate = true;

    }

    public void JoinRoomPrivate()
    {
        isPrivate = true;
        ////Instead of  entering name, Enter code first
        ////copy logic of entering from public room
        //PhotonNetwork.JoinRoom(privateRoomNameInputField.text.ToUpper());
        //MenuManager.Instance.OpenMenu("loading");
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

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

}