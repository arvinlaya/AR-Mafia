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
    public const int NIGHT_LENGHT = 40; //40 //murder, open door
    public const int DAY_DISCUSSION_LENGHT = 30; //30 // none
    public const int DAY_ACCUSE_LENGHT = 20; //20 // accuse icon
    public const int DAY_ACCUSE_DEFENSE_LENGHT = 20; //20 // none
    public const int DAY_VOTE_LENGHT = 20; //20 // guilty, not guilty
    public const int ROLE_PANEL_DURATION = 3;
    public const int GAME_START = 3;

    public GAME_PHASE GAME_STATE = GAME_PHASE.NIGHT;
    PhotonView PV;
    Role[] roles;
    TMP_Text uiTimer;
    public event Action OnPhaseChange;
    int openDoorCastTime;
    int abilityCastTime;
    private int currentTime;
    private Coroutine timerCoroutine;
    private Dictionary<Player, int> highestAccusedPlayerDict;
    private Player highestAccusedPlayer;
    private List<Player> aliveList;
    private bool firstNight;
    private int dayCount;
    private int aliveCount;

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
        ReadyManager.Instance.setRequiredReady(PhotonNetwork.PlayerList.Count());
        CooldownManager.Instance.setDoorCooldown(false);
        CooldownManager.Instance.setSkillCooldown(false);
        Instance.uiTimer = ReferenceManager.Instance.UITimer;
        Instance.PV = Instance.gameObject.GetComponent<PhotonView>();
        aliveList = new List<Player>();
        firstNight = true;
        dayCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            aliveList.Add(player);
        }
        aliveCount = aliveList.Count;

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
                //     roleCustomProps.Add("ROLE", "DETECTIVE");
                // }
                roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
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
            dayCount += 1;

            if (dayCount == 1)
            {
                PV.RPC(nameof(RPC_setAliveCount), RpcTarget.All, aliveCount);
            }

            Instance.PV.RPC(nameof(RPC_setDayCount), RpcTarget.All, dayCount);
            event_code = GameManager.EVENT_CODE.NIGHT_START;

            game_winner = checkWinCondition();
            // game_winner = GameManager.GAME_WINNER.ONGOING;

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

        PhotonNetwork.RaiseEvent((byte)event_code, phase,
                                    new RaiseEventOptions
                                    {
                                        Receivers = ReceiverGroup.All
                                    },
                                    new SendOptions { Reliability = true });
    }
    private IEnumerator SetPhase_R(object phase)
    {

        GameManager.Instance.GAME_STATE = (GameManager.GAME_PHASE)phase;
        OnPhaseChange?.Invoke();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            setAnimationSyncState(player, (GameManager.GAME_PHASE)phase);
        }

        if (aliveList.Contains(PhotonNetwork.LocalPlayer))
        {
            PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(false);
        }

        ReadyManager.Instance.setReady(true);

        if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.NIGHT)
        {
            DayCycleManager.Instance.setDayState(DayCycleManager.DAY_STATE.NIGHT);
            StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.NIGHT_PHASE_START, 2f));
        }
        else if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.DAY_DISCUSSION)
        {
            DayCycleManager.Instance.setDayState(DayCycleManager.DAY_STATE.DAY);
            StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DAY_PHASE_START, 2f));
        }
        else
        {
            DayCycleManager.Instance.setDayState(DayCycleManager.DAY_STATE.DAY);
        }

        yield return new WaitUntil(() => ReadyManager.Instance.getIsAllReady());



        InitializeTimer((byte)phase);
    }

    private void InitializeTimer(byte phase)
    {
        if (phase == (byte)GameManager.GAME_PHASE.NIGHT)
        {
            CooldownManager.Instance.setDoorCooldown(false);
            CooldownManager.Instance.setSkillCooldown(false);
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
        if (CooldownManager.Instance.getIsDoorCooldown())
        {
            CooldownManager.Instance.doorCooldownCheck(currentTime);
        }
        RefreshTimerUI();

        if (currentTime == 5)
        {
            SoundManager.Instance.playGameClip(SoundManager.TIME_ENDING, 0);
        }
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
        else
        {
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
                StartCoroutine(dayVoteStartSequence(photonEvent));
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

        resetOutsiderCount();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        int guiltyVotes = VoteManager.Instance.getGuiltyVotes();
        int innocentVotes = VoteManager.Instance.getInnocentVotes();

        if (guiltyVotes > 0 || innocentVotes > 0)
        {
            yield return StartCoroutine(PromptManager.Instance.promptEliminationVotes(VoteManager.Instance.getGuiltyVotes(), VoteManager.Instance.getInnocentVotes(), 5f));
        }

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
                yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).goBackToHouseSequence());
            }
        }
        VoteManager.Instance.resetAll();

        if (firstNight == false)
        {
            closeAllDoors();
        }

        firstNight = false;
        StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DOOR_OPEN_CLOSE, 0));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));
    }

    private IEnumerator dayDiscussionStartSequence(EventData photonEvent)
    {
        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        closeAllDoors();
        StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DOOR_OPEN_CLOSE, 0));

        yield return new WaitForSeconds(2f);
        resetAllPlayerPosition();

        yield return new WaitForSeconds(2f);

        openAllDoors();
        StartCoroutine(SoundManager.Instance.playGameClip(SoundManager.DOOR_OPEN_CLOSE, 0));


        yield return new WaitForSeconds(2f);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isDead = (bool)player.CustomProperties["IS_DEAD"];
            bool isSaved = (bool)player.CustomProperties["IS_SAVED"];

            if (isDead == true && isSaved == false)
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(player).dieSequence());
                yield return StartCoroutine(PromptManager.Instance.promptTemporary(player.NickName + " was poisoned after they had a conversation with the mafia last night.", 5f));
                yield return StartCoroutine(PromptManager.Instance.promptTemporary("The mafia lurked in the shadows and passed a night without killing a villager.", 5f));
                aliveList.Remove(player);
                if (PhotonNetwork.IsMasterClient)
                {
                    PV.RPC(nameof(RPC_setAliveCount), RpcTarget.All, aliveList.Count);
                }
            }
            else if (isDead == true && isSaved == true)
            {
                yield return StartCoroutine(PromptManager.Instance.promptTemporary("The <color=\"blue\">doctor</color> successfully saved the <color=\"red\">mafia's target</color>", 5f));
                player.SetCustomProperties(new Hashtable() { { "IS_SAVED", false } });
                player.SetCustomProperties(new Hashtable() { { "IS_DEAD", false } });
            }
            else if (isDead == false && isSaved == true)
            {
                player.SetCustomProperties(new Hashtable() { { "IS_SAVED", false } });
            }
        }

        yield return StartCoroutine(PromptManager.Instance.promptStay("The village woke up and will start discussing about the event that occured last night."));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

    }

    private IEnumerator dayAccuseStartSequence(EventData photonEvent)
    {
        resetAllPlayerPosition();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        openAllDoors();

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(PromptManager.Instance.promptStay("The village will now start voting who they think is the mafia.\n\nMinimum votes required: 2"));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

    }

    private IEnumerator dayAccuseDefenseStartSequence(EventData photonEvent)
    {
        resetAllPlayerPosition();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        Dictionary<Player, int> playerAccuseVotes = VoteManager.Instance.getPlayerAccuseVotes();
        if (playerAccuseVotes.Count > 0)
        {
            yield return StartCoroutine(PromptManager.Instance.promptAccuseVotes(playerAccuseVotes, 5f));
        }

        openAllDoors();

        yield return new WaitForSeconds(2f);
        highestAccusedPlayerDict = VoteManager.Instance.getHighestAccusedPlayer();
        highestAccusedPlayer = null;
        if (highestAccusedPlayerDict != null)
        {
            highestAccusedPlayer = highestAccusedPlayerDict.ElementAt(0).Key;
            yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).accusedSequence());
            yield return StartCoroutine(PromptManager.Instance.promptTemporary(highestAccusedPlayer.NickName + " is accused as the murderer in the village.", 5f));
            yield return StartCoroutine(PromptManager.Instance.promptStay(highestAccusedPlayer.NickName + " will now have to convince the villager that he/she is innocent."));
            yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

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
        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PromptManager.Instance.promptTemporary("The village will now make a decision if " + highestAccusedPlayer + " is guilty of the charges", 5f));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));
    }

    private GameManager.GAME_WINNER checkWinCondition()
    {
        int mafiaCount = 0;
        int villagerCount = 0;
        string role;
        foreach (Player player in aliveList)
        {
            role = (string)player.CustomProperties["ROLE"];

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

    public void removeFromAliveList(Player player)
    {
        aliveList.Remove(player);
        PlayerManager.getPlayerController(player).ignoreRaycast();
        PlayerManager.getPlayerHouseController(player).ignoreRaycast();
    }

    private void setAnimationSyncState(Player player, GameManager.GAME_PHASE phase)
    {
        Debug.Log("SETTING ANIMATION SYNC STATE");
        if (phase == GameManager.GAME_PHASE.NIGHT)
        {
            PlayerManager.getPlayerController(player).setMovementSync(false);
        }
        else
        {
            PlayerManager.getPlayerController(player).setMovementSync(true);
        }
    }

    private void closeAllDoors()
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.closeDoor();
        }
    }

    private void openAllDoors()
    {
        foreach (HouseController controller in GameObject.FindObjectsOfType<HouseController>())
        {
            controller.openDoor();
        }
    }

    private void resetAllPlayerPosition()
    {
        foreach (Player player in aliveList)
        {
            PlayerManager.getPlayerController(player).resetPlayerState();
        }
    }

    private void resetOutsiderCount()
    {
        foreach (Player player in aliveList)
        {
            CustomPropertyWrapper.setPropertyInt(player, "OUTSIDER_COUNT", 0);
        }
    }

    [PunRPC]
    private void RPC_setDayCount(int dayCount)
    {
        LogManager.Instance.updateDayCount(dayCount);
    }

    [PunRPC]
    private void RPC_setAliveCount(int aliveCount)
    {
        LogManager.Instance.updateAliveCount(aliveCount);
    }
}
