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
using System.Linq;
public class GameManager : MonoBehaviourPunCallbacks
{
    public enum EVENT_CODE : byte
    {
        REFRESH_TIMER,
        CAST_ACCUSE_VOTE,
        CAST_ELIMINATION_VOTE,
        PLAYER_KILLED,
        VILLAGER_WIN,
        MAFIA_WIN,
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

    public enum GAME_WINNER : byte
    {
        VILLAGER,
        MAFIA,
        ONGOING
    }

    public static GameManager Instance;
    public static int NIGHT_LENGHT = 10; //40 //murder, open door
    public static int DAY_DISCUSSION_LENGHT = 10; //30 // none
    public static int DAY_ACCUSE_LENGHT = 5; //20 // accuse icon
    public static int DAY_ACCUSE_DEFENSE_LENGHT = 5; //20 // none
    public static int DAY_VOTE_LENGHT = 20; //20 // guilty, not guilty
    public static int DOOR_COOLDOWN = 15; //15
    public static int ABILITY_COODLDOWN = NIGHT_LENGHT; //40
    public static int ROLE_PANEL_DURATION = 3;
    public static int GAME_START = 3;

    public GAME_PHASE GAME_STATE = GAME_PHASE.NIGHT;
    PhotonView PV;
    Role[] roles;
    TMP_Text uiTimer;

    public bool openDoorOnCooldown { get; set; }
    public bool abilityOnCooldown { get; set; }
    public event Action OnPhaseChange;
    int openDoorCastTime;
    int abilityCastTime;
    private int currentTime;
    private Coroutine timerCoroutine;
    private Dictionary<Player, int> highestAccusedPlayerDict;
    private Player highestAccusedPlayer;
    private Dictionary<Player, bool> aliveList;
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
        setDoorCooldown(false);
        setAbilityCooldown(false);
        Instance.uiTimer = ReferenceManager.Instance.UITimer;
        Instance.PV = Instance.gameObject.GetComponent<PhotonView>();
        aliveList = new Dictionary<Player, bool>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            aliveList.Add(player, true);
        }
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
    public void playerKilled_S(Player player)
    {
        GameManager.EVENT_CODE event_code = GameManager.EVENT_CODE.PLAYER_KILLED;

        PhotonNetwork.RaiseEvent((byte)event_code, player.NickName,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });
    }
    public void playerKilled_R(Player player)
    {
        aliveList[player] = false;
    }
    public void setDoorCooldown(bool value)
    {
        if (value == true)
        {
            openDoorCastTime = ReferenceManager.Instance.time;
            openDoorOnCooldown = true;
        }
        else
        {
            openDoorOnCooldown = false;
        }
    }

    public void openDoorCooldownCheck()
    {
        if ((openDoorCastTime - DOOR_COOLDOWN) <= ReferenceManager.Instance.time)
        {
            abilityOnCooldown = false;
        }
    }

    public void setAbilityCooldown(bool value)
    {
        if (value == true)
        {
            abilityCastTime = ReferenceManager.Instance.time;
            abilityOnCooldown = true;
        }
        else
        {
            abilityOnCooldown = false;
        }
    }

    public void abilityCooldownCheck()
    {
        if ((abilityCastTime - ABILITY_COODLDOWN) <= ReferenceManager.Instance.time)
        {
            abilityOnCooldown = false;
        }
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
                if (player.IsMasterClient)
                {
                    roleCustomProps.Add("ROLE", "MAFIA");
                }
                else
                {
                    roleCustomProps.Add("ROLE", "VILLAGER");
                }
                // roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
                roleCustomProps.Add("IS_DEAD", false);
                roleCustomProps.Add("IS_SAVED", false);
                roleCustomProps.Add("OUTSIDER_COUNT", 0);
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
        GameManager.GAME_WINNER game_winner;

        if ((byte)GameManager.GAME_PHASE.NIGHT == (byte)phase)
        {
            event_code = GameManager.EVENT_CODE.NIGHT_START;


            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // UNCOMMENT AFTER DEBUGGING
            // game_winner = checkWinCondition();
            game_winner = GAME_WINNER.ONGOING;

            if (game_winner == GameManager.GAME_WINNER.VILLAGER)
            {
                event_code = GameManager.EVENT_CODE.VILLAGER_WIN;
            }
            else if (game_winner == GameManager.GAME_WINNER.MAFIA)
            {
                event_code = GameManager.EVENT_CODE.MAFIA_WIN;
            }
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

        if ((byte)event_code == (byte)GameManager.EVENT_CODE.NIGHT_START)
        {
            DayCycleManager.Instance.setDayState(DayCycleManager.DAY_STATE.NIGHT);
        }
        else
        {
            DayCycleManager.Instance.setDayState(DayCycleManager.DAY_STATE.DAY);
        }
        PhotonNetwork.RaiseEvent((byte)event_code, phase,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });
    }
    private void SetPhase_R(object phase)
    {
        foreach (KeyValuePair<Player, bool> player in aliveList)
        {
            // If player is not alive
            if (player.Value == false)
            {
                continue;
            }

            Debug.Log("SETTING ANIMATION SYNC STATE");
            if ((GameManager.GAME_PHASE)phase == GameManager.GAME_PHASE.NIGHT)
            {
                PlayerManager.getPlayerController(player.Key).setMovementSync(false);
            }
            else
            {
                PlayerManager.getPlayerController(player.Key).setMovementSync(true);
            }
        }

        GameManager.Instance.GAME_STATE = (GameManager.GAME_PHASE)phase;
        OnPhaseChange?.Invoke();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["IS_DEAD"] == false)
            {
                if (highestAccusedPlayer != null && highestAccusedPlayer == player)
                {
                    continue;
                }
                PlayerManager.getPlayerController(player).resetPlayerState();
            }
        }

        InitializeTimer((byte)phase);
    }

    private void InitializeTimer(byte phase)
    {
        setDoorCooldown(false);
        setAbilityCooldown(false);
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
                if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.NIGHT)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_DISCUSSION);
                }
                else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_DISCUSSION)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_ACCUSE);
                }
                else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_ACCUSE)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE);
                }
                else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE)
                {
                    SetPhase_S(GameManager.GAME_PHASE.DAY_VOTE);
                }
                else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_VOTE)
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
        openDoorCooldownCheck();
        abilityCooldownCheck();
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
        else if (eventCode == (byte)GameManager.EVENT_CODE.VILLAGER_WIN)
        {
            Debug.Log("VILLAGER WON");
        }
        else if (eventCode == (byte)GameManager.EVENT_CODE.MAFIA_WIN)
        {
            Debug.Log("MAFIA WON");
        }
        else if (eventCode == (byte)GameManager.EVENT_CODE.CAST_ACCUSE_VOTE)
        {
            VoteManager.Instance.castAccuseVote_R((string)photonEvent.CustomData);
        }
        else if (eventCode == (byte)GameManager.EVENT_CODE.CAST_ELIMINATION_VOTE)
        {
            object[] data = (object[])photonEvent.CustomData;
            VoteManager.Instance.castEliminationVote_R((VoteManager.VOTE_CASTED)data[0], (VoteManager.VOTE_CASTED)data[1]);
        }
        else if (eventCode == (byte)GameManager.EVENT_CODE.PLAYER_KILLED)
        {
            object data = (object)photonEvent.CustomData;
            string playerName = (string)data;

            Player killedPlayer = PlayerManager.getPlayerByName(playerName);

            if (killedPlayer != null)
            {
                playerKilled_R(killedPlayer);
            }
        }
        else
        {
            GameManager.Instance.setDoorCooldown(false);

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
                StartCoroutine(dayAccuseStartSequence(photonEvent));

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
                StartCoroutine(dayAccuseDefenseStartSequence(photonEvent));
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
    private IEnumerator nightStartSequence(EventData photonEvent)
    {
        if (highestAccusedPlayer != null)
        {
            if (VoteManager.Instance.isGuilty())
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).guiltySequence());
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(PromptManager.Instance.promptTemporary("The village agreed that " + highestAccusedPlayer.NickName + " is the murderer.", 5f));
            }
            else
            {
                yield return StartCoroutine(PromptManager.Instance.promptTemporary("The village agreed that " + highestAccusedPlayer.NickName + " is innocent.", 5f));
            }
        }
        VoteManager.Instance.resetAll();
        yield return null;
        SetPhase_R((object)photonEvent.CustomData);
    }

    private IEnumerator dayDiscussionStartSequence(EventData photonEvent)
    {
        bool isSomeoneDead = false;
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }

        yield return new WaitForSeconds(2f);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["IS_DEAD"] == true && (bool)player.CustomProperties["IS_SAVED"] == false)
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(player).dieSequence());
                yield return StartCoroutine(PromptManager.Instance.promptTemporary(player.NickName + " was poisoned after they had a conversation with the mafia last night.", 5f));
                isSomeoneDead = true;
            }
        }

        if (isSomeoneDead == false)
        {
            yield return StartCoroutine(PromptManager.Instance.promptTemporary("The mafia lurked in the shadows and passed a night without killing a villager.", 5f));
        }

        yield return StartCoroutine(PromptManager.Instance.promptStay("The village woke up and will start discussing about the event that occured last night."));

        SetPhase_R((object)photonEvent.CustomData);
    }

    private IEnumerator dayAccuseStartSequence(EventData photonEvent)
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(PromptManager.Instance.promptStay("The village will now start voting who they think is the mafia.\n\nMinimum votes required: 2"));

        SetPhase_R((object)photonEvent.CustomData);
    }

    private IEnumerator dayAccuseDefenseStartSequence(EventData photonEvent)
    {
        VoteManager.Instance.showVotes();
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }

        yield return new WaitForSeconds(2f);
        highestAccusedPlayerDict = VoteManager.Instance.getHighestAccusedPlayer();
        highestAccusedPlayer = null;
        if (highestAccusedPlayerDict != null)
        {
            highestAccusedPlayer = highestAccusedPlayerDict.ElementAt(0).Key;
            yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).accusedSequence());
            yield return StartCoroutine(PromptManager.Instance.promptTemporary(highestAccusedPlayer.NickName + " is accused as the murderer in the village.", 5f));
            yield return StartCoroutine(PromptManager.Instance.promptStay(highestAccusedPlayer.NickName + " will now have to convince the villager that he/she is innocent."));
            SetPhase_R((object)photonEvent.CustomData);

            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return StartCoroutine(PromptManager.Instance.promptTemporary("The village did not agreed upon who they think is the mafia.", 5f));
            yield return StartCoroutine(PromptManager.Instance.promptStay("The village will proceed to sleep another night."));
            if (PhotonNetwork.IsMasterClient)
            {
                SetPhase_S(GameManager.GAME_PHASE.NIGHT);
            }
        }
    }

    private IEnumerator dayVoteStartSequence(EventData photonEvent)
    {
        yield return StartCoroutine(PromptManager.Instance.promptTemporary("The village will now make a decision if " + highestAccusedPlayer + " is guilty of the charges", 5f));

        SetPhase_R((object)photonEvent.CustomData);
    }

    private GameManager.GAME_WINNER checkWinCondition()
    {
        int mafiaCount = 0;
        int villagerCount = 0;
        string role;
        foreach (KeyValuePair<Player, bool> entry in aliveList)
        {
            if (entry.Value == false)
            {
                continue;
            }
            role = (string)entry.Key.CustomProperties["ROLE"];

            if (role.Trim() == "MAFIA")
            {
                mafiaCount++;
            }
            else
            {
                villagerCount++;
            }
        }

        if (mafiaCount <= 0)
        {
            return GameManager.GAME_WINNER.VILLAGER;
        }
        else if (mafiaCount >= villagerCount)
        {
            return GameManager.GAME_WINNER.MAFIA;
        }
        else
        {
            return GameManager.GAME_WINNER.ONGOING;
        }
    }

    public void rotateToCamera(Transform toRotate, Transform target)
    {
        toRotate.LookAt(target);
    }

    public void rotateToCamera(Transform toRotate, Transform target, float offsetX, float offsetY, float offsetZ)
    {
        toRotate.LookAt(target);
        toRotate.rotation *= Quaternion.Euler(offsetX, offsetY, offsetZ);
    }
}
