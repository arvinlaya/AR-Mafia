using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject burger;
    [SerializeField]
    private GameObject dayCount;
    [SerializeField]
    private GameObject aliveCounter;
    [SerializeField]
    private GameObject gamePhase;
    [SerializeField]
    private GameObject timer;
    private int maxPlayerCount;

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
        maxPlayerCount = PhotonNetwork.PlayerList.Length;
    }

    public void setDayCount(int count)
    {
        this.dayCount.GetComponent<TMP_Text>().SetText($"DAY {count}");
    }

    public void setAliveCount(int count)
    {
        this.aliveCounter.GetComponent<TMP_Text>().SetText($"{count}/{maxPlayerCount}");
    }

    public void setGamePhase(byte phase)
    {
        TMP_Text text = this.gamePhase.GetComponent<TMP_Text>();

        switch (phase)
        {
            case (byte)GameManager.GAME_PHASE.NIGHT:
                text.SetText("NIGHT TIME");
                break;

            case (byte)GameManager.GAME_PHASE.DAY_DISCUSSION:
                text.SetText("HOLD DISCUSSION");
                break;

            case (byte)GameManager.GAME_PHASE.DAY_ACCUSE:
                text.SetText("ACCUSATION");
                break;

            case (byte)GameManager.GAME_PHASE.DAY_ACCUSE_DEFENSE:
                text.SetText("DEFEND THE ACCUSED");
                break;

            case (byte)GameManager.GAME_PHASE.DAY_VOTE:
                text.SetText("VOTING");
                break;
        }
    }

    public void setTime(string time)
    {
        this.timer.GetComponent<TMP_Text>().SetText($"{time}s");
    }
}
