using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LogManager : MonoBehaviour
{
    private PhotonView PV;
    public static LogManager Instance;
    [SerializeField] public GameObject content;
    [SerializeField] public GameObject log;
    [SerializeField] public GameObject scrollView;
    [SerializeField] public GameObject scrollBar;


    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    public void addMafiaAction(string source, string targetPlayer)
    {
        PV.RPC(nameof(RPC_addMafiaAction), RpcTarget.All, source, targetPlayer);

        scrollToBottom();
    }

    [PunRPC]
    public void RPC_addMafiaAction(string source, string targetPlayer)
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"] == "MAFIA")
        {
            string message = $"<color=\"red\">{source}</color> is targeting <color=\"blue\">{targetPlayer}</color>";
            GameObject logItem = Instantiate(log, content.transform);

            logItem.GetComponent<TMP_Text>().SetText(message);
        }

        scrollToBottom();
    }
    public void addDoctorAction(string source, string targetPlayer)
    {
        string message = $"<color=\"blue\">{source}</color> is saving <color=\"green\">{targetPlayer}</color>";
        GameObject logItem = Instantiate(log, content.transform);

        logItem.GetComponent<TMP_Text>().SetText(message);

        scrollToBottom();
    }
    public void addDetectiveAction(string source, string targetPlayer)
    {
        string message = $"<color=\"yellow\">{source}</color> is investigating <color=\"red\">{targetPlayer}</color>";
        GameObject logItem = Instantiate(log, content.transform);

        logItem.GetComponent<TMP_Text>().SetText(message);

        scrollToBottom();
    }

    public void openDoorAction(string source, string targetPlayer)
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"] == "MAFIA")
        {
            PV.RPC(nameof(RPC_openDoorAction), RpcTarget.All, source, targetPlayer);
        }
        else
        {
            string message = $"<color=\"yellow\">{source}</color> is now entering <color=\"red\">{targetPlayer}'s</color> house";
            GameObject logItem = Instantiate(log, content.transform);

            logItem.GetComponent<TMP_Text>().SetText(message);
        }

        scrollToBottom();
    }

    [PunRPC]
    public void RPC_openDoorAction(string source, string targetPlayer)
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"] == "MAFIA")
        {
            string message = $"<color=\"yellow\">{source}</color> is now entering <color=\"red\">{targetPlayer}'s</color> house";
            GameObject logItem = Instantiate(log, content.transform);

            logItem.GetComponent<TMP_Text>().SetText(message);

            scrollToBottom();
        }
    }

    public void skillCooldown()
    {
        string message = $"You can only use your ability once per night.";
        GameObject logItem = Instantiate(log, content.transform);

        logItem.GetComponent<TMP_Text>().SetText(message);

        scrollToBottom();
    }
    public void openDoorCooldown(int remaining)
    {
        string message = $"You must wait <color=\"yellow\">{remaining}</color> seconds to open another door";
        GameObject logItem = Instantiate(log, content.transform);

        logItem.GetComponent<TMP_Text>().SetText(message);

        scrollToBottom();
    }

    public void scrollToBottom()
    {
        scrollBar.GetComponent<Scrollbar>().value = 0;
    }

}
