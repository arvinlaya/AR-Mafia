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
    public const int NIGHT_LENGHT = 5; //40 //murder, open door
    public const int DAY_DISCUSSION_LENGHT = 5; //30 // none
    public const int DAY_ACCUSE_LENGHT = 5; //20 // accuse icon
    public const int DAY_ACCUSE_DEFENSE_LENGHT = 5; //20 // none
    public const int DAY_VOTE_LENGHT = 5; //20 // guilty, not guilty

    public GAME_PHASE GAME_STATE = GAME_PHASE.NIGHT;
    PhotonView PV;
    Role[] roles;
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
    private string localRole;
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
        Instance.PV = Instance.gameObject.GetComponent<PhotonView>();
        aliveList = new List<Player>();
        firstNight = true;
        dayCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            aliveList.Add(player);
        }
        aliveCount = aliveList.Count;

        // Invoke("startGame", GAME_START);
    }



    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived -= Instance.OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += Instance.OnEvent;
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.sceneLoaded -= Instance.OnSceneLoad;
            SceneManager.sceneLoaded += Instance.OnSceneLoad;
        }
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.IsMasterClient && scene.name == "Game")
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
                //     roleCustomProps.Add("ROLE", "DOCTOR");
                // }
                // else
                // {
                //     roleCustomProps.Add("ROLE", "VILLAGER");
                // }
                roleCustomProps.Add("ROLE", roles[index].ROLE_TYPE);
                roleCustomProps.Add("IS_DEAD", false);
                roleCustomProps.Add("IS_SAVED", false);
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
            Instance.PV.RPC(nameof(RPC_syncDayCount), RpcTarget.All, ++dayCount);

            if (dayCount == 1)
            {
                PV.RPC(nameof(RPC_setAliveCount), RpcTarget.All, aliveCount);
            }

            event_code = GameManager.EVENT_CODE.NIGHT_START;

            // game_winner = checkWinCondition();
            game_winner = GameManager.GAME_WINNER.ONGOING;

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

        if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.NIGHT)
        {
            if ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"] == "DOCTOR")
            {
                PlayerController localController = PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer);
                localController.nightSaveInterval += 1;
                if (localController.nightSaveInterval == 2)
                {
                    localController.previousSaved = PhotonNetwork.LocalPlayer;

                    localController.nightSaveInterval = 0;
                }
            }

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

        ReadyManager.Instance.setReady(true);

        yield return new WaitUntil(() => ReadyManager.Instance.getIsAllReady());
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.CustomProperties["IS_INSTANTIATED"] == null)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "IS_INSTANTIATED", true } });
        }

        yield return StartCoroutine(UIManager.Instance.setGamePhase((byte)phase));

        InitializeTimer((byte)phase);

        UIManager.Instance.setDayCount(dayCount, (byte)phase);

        ReadyManager.Instance.resetReady();
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

        if (PhotonNetwork.IsMasterClient)
        {
            RefreshTimer_S(currentTime);
        }

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
            timerCoroutine = StartCoroutine(Timer());
        }
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
        UIManager.Instance.setTime(currentTime.ToString("00"));

        if (currentTime == 5)
        {
            SoundManager.Instance.playGameClip(SoundManager.TIME_ENDING, 0);
        }
    }
    public void OnEvent(EventData photonEvent)
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
            VoteManager.Instance.castEliminationVote_R((VoteManager.VOTE_CASTED)data[0], (VoteManager.VOTE_CASTED)data[1], (string)data[2]);
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
        int players = aliveCount <= 5 ? 5 : aliveCount;
        int indexOffset = 4;

        switch (role)
        {
            case "VILLAGER":
                ReferenceManager.Instance.villagerPanels[0].SetActive(true);
                ReferenceManager.Instance.villagerPanels[players - indexOffset].SetActive(true);
                break;

            case "DOCTOR":
                ReferenceManager.Instance.doctorPanels[0].SetActive(true);
                ReferenceManager.Instance.doctorPanels[players - indexOffset].SetActive(true);
                break;

            case "MAFIA":
                ReferenceManager.Instance.mafiaPanels[0].SetActive(true);
                ReferenceManager.Instance.mafiaPanels[players - indexOffset].SetActive(true);
                break;

            case "DETECTIVE":
                ReferenceManager.Instance.detectivePanels[0].SetActive(true);
                ReferenceManager.Instance.detectivePanels[players - indexOffset].SetActive(true);
                break;
        }
    }

    public void startGame()
    {
        ReferenceManager.Instance.hideableUI.alpha = 1;

        if (PhotonNetwork.IsMasterClient)
        {
            SetPhase_S(GameManager.GAME_PHASE.NIGHT);
        }


    }
    private IEnumerator nightStartSequence(EventData photonEvent)
    {
        if (firstNight)
        {
            string role = PhotonNetwork.LocalPlayer.CustomProperties["ROLE"].ToString().Trim();

            yield return StartCoroutine(PromptManager.Instance.revealPreRolePanel(role));
        }
        resetOutsiderCount();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        StartCoroutine(PromptManager.Instance.promptRoleMessage((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"], getMafiaCount()));
        yield return new WaitForSeconds(1f);

        int guiltyVotes = VoteManager.Instance.getGuiltyVotes();
        int innocentVotes = VoteManager.Instance.getInnocentVotes();

        if (highestAccusedPlayer != null)
        {
            if (guiltyVotes > 0 || innocentVotes > 0)
            {
                yield return StartCoroutine(PromptManager.Instance.promptNoDelay($"Vote results for {highestAccusedPlayer.NickName}:"));
                yield return StartCoroutine(PromptManager.Instance.promptEliminationVotes(highestAccusedPlayer.NickName, 7f));
            }

            if (VoteManager.Instance.isGuilty())
            {
                yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).guiltySequence());
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(PromptManager.Instance.promptWithDelay("The village agreed that " + highestAccusedPlayer.NickName + " is the murderer.", 5f));
            }
            else
            {
                yield return StartCoroutine(PromptManager.Instance.promptWithDelay("The village agreed that " + highestAccusedPlayer.NickName + " is innocent.", 5f));
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

        List<Player> targetedPlayers = new List<Player>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isDead = (bool)player.CustomProperties["IS_DEAD"];
            bool isSaved = (bool)player.CustomProperties["IS_SAVED"];

            if (isDead == true)
            {
                targetedPlayers.Add(player);
            }
            else if (isSaved == true)
            {
                player.SetCustomProperties(new Hashtable() { { "IS_SAVED", false } });
            }
        }

        if (targetedPlayers.Count != 1)
        {
            yield return StartCoroutine(PromptManager.Instance.alertPromptWithDelay("The mafia(s) got disorganized and the murder attempt failed.", 5f));
        }
        else if ((bool)targetedPlayers[0].CustomProperties["IS_SAVED"] == true)
        {
            yield return StartCoroutine(PromptManager.Instance.alertPromptWithDelay("The <color=\"blue\">doctor</color> successfully saved the <color=\"red\">mafia's target</color>", 5f));
            targetedPlayers[0].SetCustomProperties(new Hashtable() { { "IS_SAVED", false } });
            targetedPlayers[0].SetCustomProperties(new Hashtable() { { "IS_DEAD", false } });
        }
        else
        {
            yield return StartCoroutine(PlayerManager.getPlayerController(targetedPlayers[0]).dieSequence());
            yield return StartCoroutine(PromptManager.Instance.alertPromptWithDelay(targetedPlayers[0].NickName + " was poisoned after they had a conversation with the mafia last night.", 5f));
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC(nameof(RPC_setAliveCount), RpcTarget.All, aliveList.Count);
            }
        }

        yield return StartCoroutine(PromptManager.Instance.promptNoDelay("Want to claim a role? Accuse someone? Confess? <b>Do that now!<b>"));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

    }

    private IEnumerator dayAccuseStartSequence(EventData photonEvent)
    {
        resetAllPlayerPosition();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        openAllDoors();

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(PromptManager.Instance.promptNoDelay("Now it is time to nominate someone!\n<b>Select a player and cast your vote.<b>\n<b><color=\"red\">Minimum votes required: 2</color>"));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

    }

    private IEnumerator dayAccuseDefenseStartSequence(EventData photonEvent)
    {
        resetAllPlayerPosition();

        PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer).disableControls(true);

        yield return new WaitForSeconds(1f);

        openAllDoors();

        yield return new WaitForSeconds(2f);
        highestAccusedPlayerDict = VoteManager.Instance.getHighestAccusedPlayer();
        highestAccusedPlayer = null;
        if (highestAccusedPlayerDict != null)
        {
            highestAccusedPlayer = highestAccusedPlayerDict.ElementAt(0).Key;
            yield return StartCoroutine(PlayerManager.getPlayerController(highestAccusedPlayer).accusedSequence());
            yield return StartCoroutine(PromptManager.Instance.promptWithDelay(highestAccusedPlayer.NickName + " is accused as the murderer in the village.", 5f));
            yield return StartCoroutine(PromptManager.Instance.promptNoDelay(highestAccusedPlayer.NickName + " has been called to the stand.\n<b>How do you plea " + highestAccusedPlayer.NickName + "?"));
            yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));

            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return StartCoroutine(PromptManager.Instance.promptWithDelay("The village did not agreed upon who they think is the mafia.", 5f));
            yield return StartCoroutine(PromptManager.Instance.promptNoDelay("The village will proceed to sleep another night."));
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

        yield return StartCoroutine(PromptManager.Instance.promptWithDelay("The village will now make a decision if " + highestAccusedPlayer + " is guilty of the charges", 5f));
        yield return StartCoroutine(PromptManager.Instance.promptWithDelay("It is judgement time! " + highestAccusedPlayer.NickName + "has been accused.\n<b>Press " + highestAccusedPlayer.NickName + " to cast your vote</b>", 5f));

        yield return StartCoroutine(SetPhase_R((object)photonEvent.CustomData));
    }

    private GameManager.GAME_WINNER checkWinCondition()
    {
        int mafiaCount = getMafiaCount();
        int villagerCount = 0;
        string role;

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
            PlayerManager.getPlayerHouseController(player).outsiderCount = 0;
        }
    }

    [PunRPC]
    private void RPC_syncDayCount(int dayCount)
    {
        this.dayCount = dayCount;
    }

    [PunRPC]
    private void RPC_setAliveCount(int aliveCount)
    {
        UIManager.Instance.setAliveCount(aliveCount);
    }

    private int getMafiaCount()
    {
        int mafiaCount = 0;

        foreach (Player player in aliveList)
        {
            if (player.CustomProperties["ROLE"].ToString().Trim() == "MAFIA")
            {
                mafiaCount++;
            }
        }

        return mafiaCount;
    }

    public List<Player> getAliveList()
    {
        return this.aliveList;
    }
}
