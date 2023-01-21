using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    TMP_Text uiTimer;
    private int currentTime;
    private Coroutine timerCoroutine;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        uiTimer = ReferenceManager.Instance.UITimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPhase_S(GameManager.GAME_PHASE.DAY_ACCUSE);
            SpawnManager.Instance.SpawnPlayersAndHouses();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)GameManager.EVENT_CODE.REFRESH_TIMER)
        {
            RefreshTimer_R((object)photonEvent.CustomData);
        }
        else if (eventCode == (byte)GameManager.EVENT_CODE.NIGHT_START ||
                eventCode == (byte)GameManager.EVENT_CODE.DAY_DISCUSSION_START ||
                eventCode == (byte)GameManager.EVENT_CODE.DAY_ACCUSE_START ||
                eventCode == (byte)GameManager.EVENT_CODE.DAY_ACCUSE_DEFENSE_START ||
                eventCode == (byte)GameManager.EVENT_CODE.DAY_VOTE_START)
        {
            SetPhase_R((object)photonEvent.CustomData);
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
        InitializeTimer((byte)phase);
    }

    private void RefreshTimer_R(object data)
    {
        currentTime = (int)data;
        RefreshTimerUI();
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

    private void EndPhase()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
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
}
