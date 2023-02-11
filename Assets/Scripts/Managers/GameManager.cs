using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using System;
public class GameManager : MonoBehaviourPunCallbacks
{
    public enum EVENT_CODE : byte
    {
        REFRESH_TIMER,
        DAY_DISCUSSION_START,
        DAY_ACCUSE_START,
        DAY_ACCUSE_DEFENSE_START,
        DAY_VOTE_START,
        NIGHT_START,
        PHASE_END
    }

    public enum GAME_PHASE : byte
    {
        DAY_DISCUSSION,
        DAY_ACCUSE,
        DAY_ACCUSE_DEFENSE,
        DAY_VOTE,
        NIGHT
    }

    public static GameManager Instance;
    public static int NIGHT_LENGHT = 40; //40 //murder, open door
    public static int DAY_DISCUSSION_LENGHT = 30; //30 // none
    public static int DAY_ACCUSE_LENGHT = 20; //20 // accuse icon
    public static int DAY_ACCUSE_DEFENSE_LENGHT = 20; //20 // none
    public static int DAY_VOTE_LENGHT = 20; //20 // guilty, not guilty
    public static int DOOR_COOLDOWN = 15; //15
    public static int ABILITY_COODLDOWN = NIGHT_LENGHT; //40

    public static int ROLE_PANEL_DURATION = 3;

    public static int GAME_START = 3;

    public static GAME_PHASE GAME_STATE = GAME_PHASE.NIGHT;
    PhotonView PV;
    Role[] roles;
    TMP_Text uiTimer;
    Player accusedPlayer;
    public bool onDoorCooldown { get; set; }
    public bool onAbilityCooldown { get; set; }
    public event Action OnPhaseChange;
    int openDoorTime;
    int abilityTime;
    private int currentTime;
    private Coroutine timerCoroutine;

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
        onDoorCooldown = false;
        onAbilityCooldown = false;
        Instance.uiTimer = ReferenceManager.Instance.UITimer;
        Instance.PV = Instance.gameObject.GetComponent<PhotonView>();
        Invoke("removeDisplayRole", ROLE_PANEL_DURATION);
        Invoke("startGame", GAME_START);
    }

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;

        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void setDoorCooldown()
    {
        openDoorTime = ReferenceManager.Instance.time;
        onDoorCooldown = true;
    }

    public bool isDoorCooldown()
    {
        if ((openDoorTime - DOOR_COOLDOWN) >= ReferenceManager.Instance.time)
        {
            onDoorCooldown = false;
        }
        return onDoorCooldown;

    }

    public void setAbilityCooldown()
    {
        abilityTime = ReferenceManager.Instance.time;

    }

    public bool isAbilityCooldown()
    {
        if ((abilityTime - ABILITY_COODLDOWN) >= ReferenceManager.Instance.time)
        {
            onAbilityCooldown = false;
        }
        return onAbilityCooldown;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // REMOVE DEFAULT PLAYER COUNT AFTER DEBUGGING
            // int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            int playerCount = 5;
            generateRoles(playerCount, out roles);
            int index = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Hashtable roleCustomProps = new Hashtable();

                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // REMOVE MASTERCLIENT = MAFIA ROLE AFTER DEBUGGING
                // if (player.IsMasterClient)
                // {
                //     roleCustomProps.Add("ROLE", "MAFIA");
                // }
                // else
                // {
                //     roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
                // }
                roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
                roleCustomProps.Add("IS_DEAD", false);
                roleCustomProps.Add("IS_SAVED", false);
                roleCustomProps.Add("VOTE_VALUE", 0);
                roleCustomProps.Add("VOTED", "");
                roleCustomProps.Add("ACCUSE_VOTE_COUNT", 0);
                roleCustomProps.Add("GUILTY_VOTE", 0);
                roleCustomProps.Add("INNOCENT_VOTE", 0);
                player.SetCustomProperties(roleCustomProps);
                index++;
            }
        }

    }

    void generateRoles(int playerCount, out Role[] rolesArray)
    {
        Debug.Log("Player count: " + playerCount);
        if (playerCount == 5)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Detective(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 6)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Detective(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 7)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Detective(),
                                new Mafia(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        else if (playerCount == 8)
        {
            rolesArray = new Role[] { new Villager(),
                                new Villager(),
                                new Villager(),
                                new Villager(),
                                new Detective(),
                                new Mafia(),
                                new Mafia(),
                                new Doctor() };
            shuffleArray(rolesArray, rolesArray.Length);
            return;
        }
        rolesArray = null;
        Debug.Log("PLAYER COUNT ERROR");
    }

    private void shuffleArray(Role[] roles, int arraySize)
    {
        System.Random random = new System.Random();
        for (int x = 0; x < arraySize; x++)
        {
            swap(roles, x, x + random.Next(arraySize - x));
        }
    }

    private void swap(Role[] arr, int a, int b)
    {
        Role temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }

    private void SetPhase_S(object phase)
    {
        GameManager.EVENT_CODE event_code = 0;
        object data = phase;

        if ((byte)GameManager.GAME_PHASE.NIGHT == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.NIGHT_START;
        }
        else if ((byte)GameManager.GAME_PHASE.DAY_DISCUSSION == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.DAY_DISCUSSION_START;
        }
        else if ((byte)GameManager.GAME_PHASE.DAY_ACCUSE == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.DAY_ACCUSE_START;
        }
        else if ((byte)GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.DAY_ACCUSE_DEFENSE_START;
        }
        else if ((byte)GameManager.GAME_PHASE.DAY_VOTE == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.DAY_VOTE_START;
        }


        PhotonNetwork.RaiseEvent((byte)event_code, data,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });
    }
    private void SetPhase_R(object phase)
    {
        GameManager.GAME_STATE = (GameManager.GAME_PHASE)phase;
        OnPhaseChange?.Invoke();
        InitializeTimer((byte)phase);
    }

    private void InitializeTimer(byte phase)
    {
        if (phase == (byte)GameManager.GAME_PHASE.NIGHT)
        {
            currentTime = GameManager.NIGHT_LENGHT;
            Debug.Log("NIGHT STARTS");
        }
        else if (phase == (byte)GameManager.GAME_PHASE.DAY_DISCUSSION)
        {


            currentTime = GameManager.DAY_DISCUSSION_LENGHT;
            Debug.Log("DAY DISCUSSION STARTS");
        }
        else if (phase == (byte)GameManager.GAME_PHASE.DAY_ACCUSE)
        {
            currentTime = GameManager.DAY_ACCUSE_LENGHT;
            Debug.Log("DAY ACCUSE STARTS");
        }
        else if (phase == (byte)GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE)
        {
            currentTime = GameManager.DAY_ACCUSE_DEFENSE_LENGHT;
            Debug.Log("DAY ACCUSE DEFENSE STARTS");
        }
        else if (phase == (byte)GameManager.GAME_PHASE.DAY_VOTE)
        {
            currentTime = GameManager.DAY_VOTE_LENGHT;
            Debug.Log("DAY VOTE STARTS");
        }

        timerCoroutine = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);

        currentTime -= 1;

        if (currentTime <= 0)
        {
            timerCoroutine = null;
            //RaiseEvent PHASE_END
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.GAME_STATE == GameManager.GAME_PHASE.NIGHT)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_DISCUSSION);
                }
                else if (GameManager.GAME_STATE == GameManager.GAME_PHASE.DAY_DISCUSSION)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_ACCUSE);
                }
                else if (GameManager.GAME_STATE == GameManager.GAME_PHASE.DAY_ACCUSE)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE);
                }
                else if (GameManager.GAME_STATE == GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_VOTE);
                }
                else if (GameManager.GAME_STATE == GameManager.GAME_PHASE.DAY_VOTE)
                {
                    SetPhase_S(GameManager.GAME_PHASE.NIGHT);
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                RefreshTimer_S(currentTime);
            }
            timerCoroutine = StartCoroutine(Timer());

        }
    }
    private void RefreshTimerUI()
    {
        uiTimer.text = currentTime.ToString("00");
    }
    private void RefreshTimer_S(object data)
    {
        PhotonNetwork.RaiseEvent((byte)GameManager.EVENT_CODE.REFRESH_TIMER, data,
                                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                    new SendOptions { Reliability = true });
    }
    private void RefreshTimer_R(object data)
    {
        currentTime = (int)data;
        RefreshTimerUI();
    }
    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)GameManager.EVENT_CODE.REFRESH_TIMER)
        {
            ReferenceManager.Instance.time = (int)photonEvent.CustomData;
            RefreshTimer_R((object)photonEvent.CustomData);
        }
        else
        {
            GameManager.Instance.onDoorCooldown = false;

            if (eventCode == (byte)GameManager.EVENT_CODE.NIGHT_START)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                {
                    gameObject.SetActive(false);
                }
                StartCoroutine(nightStartSequence(photonEvent));

            }
            else if (eventCode == (byte)GameManager.EVENT_CODE.DAY_DISCUSSION_START)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                {
                    gameObject.SetActive(false);
                }
                StartCoroutine(dayDiscussionStartSequence(photonEvent));
            }
            else if (eventCode == (byte)GameManager.EVENT_CODE.DAY_ACCUSE_START)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                {
                    gameObject.SetActive(false);
                }
                SetPhase_R((object)photonEvent.CustomData);

            }
            else if (eventCode == (byte)GameManager.EVENT_CODE.DAY_ACCUSE_DEFENSE_START)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                {
                    gameObject.SetActive(false);
                }
                StartCoroutine(dayAccuseDefenseStartSequence(photonEvent));
            }
            else if (eventCode == (byte)GameManager.EVENT_CODE.DAY_VOTE_START)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
                {
                    gameObject.SetActive(false);
                }
                SetPhase_R((object)photonEvent.CustomData);

            }
        }
    }
    private void EndPhase()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    public void activateDisplayRole(string role)
    {
        switch (role)
        {
            case "VILLAGER":
                ReferenceManager.Instance.rolePanels[0].SetActive(true);
                break;

            case "DOCTOR":
                ReferenceManager.Instance.rolePanels[1].SetActive(true);
                break;

            case "MAFIA":
                ReferenceManager.Instance.rolePanels[2].SetActive(true);
                break;

            case "DETECTIVE":
                ReferenceManager.Instance.rolePanels[3].SetActive(true);
                break;
        }
    }
    private void removeDisplayRole()
    {
        Destroy(ReferenceManager.Instance.panelParent);
    }

    private void startGame()
    {
        Instance.PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            SetPhase_S(GameManager.GAME_PHASE.NIGHT);
        }
    }




    private IEnumerator dayDiscussionStartSequence(EventData photonEvent)
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }

        yield return new WaitForSeconds(2f);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["IS_DEAD"])
            {
                if ((bool)player.CustomProperties["IS_SAVED"] == false)
                {
                    yield return StartCoroutine(PlayerManager.getPlayerController(player).dieSequence());
                    yield return StartCoroutine(PromptManager.Instance.promptMurdered(player));
                    yield return StartCoroutine(PromptManager.Instance.promptDayDiscussionPhase(player));
                }
            }
        }

        SetPhase_R((object)photonEvent.CustomData);
    }

    private IEnumerator dayAccuseDefenseStartSequence(EventData photonEvent)
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }

        yield return new WaitForSeconds(2f);
        Player highestAccuseVote = null;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((int)player.CustomProperties["ACCUSE_VOTE_COUNT"] > 0)
            {
                if (highestAccuseVote == null)
                {
                    highestAccuseVote = player;
                }
                else
                {
                    highestAccuseVote = higherAccuseVote(highestAccuseVote, player);
                }
            }

        }

        if (highestAccuseVote != null)
        {

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (highestAccuseVote.NickName != player.NickName)
                {
                    if ((int)highestAccuseVote.CustomProperties["ACCUSE_VOTE_COUNT"] == (int)player.CustomProperties["ACCUSE_VOTE_COUNT"])
                    {
                        highestAccuseVote = null;
                    }
                }

            }
            if (highestAccuseVote != null)
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(highestAccuseVote).accusedSequence());
                yield return StartCoroutine(PromptManager.Instance.promptAccused(highestAccuseVote));
                yield return StartCoroutine(PromptManager.Instance.promptAccusedPhase(highestAccuseVote));
            }
        }
        accusedPlayer = highestAccuseVote;
        yield return new WaitForSeconds(2f);

        SetPhase_R((object)photonEvent.CustomData);

    }

    private IEnumerator nightStartSequence(EventData photonEvent)
    {
        if (accusedPlayer != null)
        {
            if ((int)accusedPlayer.CustomProperties["ACCUSE_VOTE_COUNT"] > 0)
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(accusedPlayer).guiltySequence());
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(PromptManager.Instance.promptGuilty(accusedPlayer));

            }
        }


        SetPhase_R((object)photonEvent.CustomData);
    }

    private Player higherAccuseVote(Player player1, Player player2)
    {
        int player1VoteCount = (int)player1.CustomProperties["ACCUSE_VOTE_COUNT"];
        int player2VoteCount = (int)player2.CustomProperties["ACCUSE_VOTE_COUNT"];
        if (player1VoteCount > player2VoteCount)
        {
            return player1;
        }
        else
        {
            return player2;
        }
    }
    public void rotateToCamera(GameObject toRotate, GameObject camera)
    {
        toRotate.transform.LookAt(camera.transform);
    }
}
