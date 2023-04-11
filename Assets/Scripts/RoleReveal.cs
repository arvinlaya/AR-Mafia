using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class RoleReveal : MonoBehaviour, IOnEventCallback
{
    private const byte UPDATE_TIMER = 20;
    private const byte COUNTDOWN_FINISH = 21;
    private const byte SET_READY = 22;

    private int readyCount;
    private int requiredReady;
    private int time;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject parent;

    void Awake()
    {
        readyCount = 0;
        requiredReady = PhotonNetwork.CurrentRoom.PlayerCount;
        time = 5;
    }
    void Start()
    {
        PhotonNetwork.RaiseEvent(
            SET_READY,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true });

        StartCoroutine(nameof(startTimer));
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case SET_READY:
                if (PhotonNetwork.IsMasterClient)
                {
                    readyCount += 1;
                }
                break;

            case UPDATE_TIMER:
                object[] data = (object[])photonEvent.CustomData;
                time = (int)data[0];
                countdownText.SetText($"Game begins in {time}...");
                break;

            case COUNTDOWN_FINISH:
                GameManager.Instance.startGame();
                Destroy(parent);
                break;
        }
    }

    public IEnumerator startTimer()
    {
        yield return new WaitUntil(() => readyCount >= requiredReady);

        if (PhotonNetwork.IsMasterClient)
        {
            yield return StartCoroutine(timer());
        }
    }

    private IEnumerator timer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(1f);

            time -= 1;
            object[] data = new object[] { time };

            PhotonNetwork.RaiseEvent(
                UPDATE_TIMER,
                data,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true });


            if (time <= 0)
            {
                PhotonNetwork.RaiseEvent(
            COUNTDOWN_FINISH,
            data,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
            }
            else
            {
                yield return StartCoroutine(timer());
            }
        }
    }

    public void RPC_setReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            readyCount += 1;
        }
    }
}
