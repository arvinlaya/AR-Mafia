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
            InitializeTimer("NIGHT");
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

    private void InitializeTimer(string phase)
    {
        if (phase == "NIGHT")
        {
            currentTime = GameManager.NIGHT_LENGHT;
        }
        else if (phase == "DAY")
        {
            currentTime = GameManager.DAY_DISCUSSION_LENGHT;
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
        }
        else
        {
            RefreshTimer_S(currentTime);
            timerCoroutine = StartCoroutine(Timer());
        }
    }
}
