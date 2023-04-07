//ERR: Null Exception, chatField object

using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
//using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Realtime;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    //[SerializeField] GameObject joinChatbutton;
    [SerializeField] GameObject chatPanel;
    ChatClient chatClient;
    bool isConnected;
    [SerializeField] string username;
    [SerializeField] string currentChat;

    [SerializeField] string privateReceiver = "";
    [SerializeField] TMP_InputField chatField;
    //[SerializeField] TMP_Text chatDisplay;
    [SerializeField] TMP_Text chatDisplayItemPrefab2;

    [SerializeField] GameObject firstChatMessageForMafia;

    [SerializeField] Transform chatDisplayContent;
    Player player;

    private string myChannelName;

    bool isMafia = false;

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    public void SubmitPublicChatOnClick()
    {
        if (privateReceiver == "" && currentChat != "")
        {
            if (currentChat.Contains("/m"))
            {
                chatClient.PublishMessage("MafiaCH", currentChat.Replace("/m", "(MAFIA)\n"));
            }
            else
            {
                chatClient.PublishMessage(myChannelName, currentChat);
            }

            chatField.text = "";
            currentChat = "";
        }
    }

    public void ChatConnectOnClick()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(username));
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        //foreach (Transform trans in chatDisplayContent)
        //{
        //    Destroy(trans.gameObject);
        //}

        isConnected = true;

        myChannelName = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(new string[] { myChannelName });

        //TODO Gawing "PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ROLE")" pag 'di na pang demo
        //if (PhotonNetwork.LocalPlayer.NickName.ToLower().Contains("mf")) isMafia = true;

        if (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"].ToString().Trim() == "MAFIA")
        {
            isMafia = true;
        }

        if (isMafia)
        {
            chatClient.Subscribe(new string[] { "MafiaCH" });
            firstChatMessageForMafia.SetActive(true);
        }
        else firstChatMessageForMafia.SetActive(false);
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }


    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //if (
        //    PhotonNetwork.LocalPlayer.CustomProperties["ROLE"].ToString() == "MAFIA"
        //    &&
        //    messages[0].toString().Contains("(MAFIA)")
        //    )
        if (
            isMafia
            &&
            messages[0].ToString().Contains("(MAFIA)")
            )
        {
            Instantiate(chatDisplayItemPrefab2, chatDisplayContent).GetComponent<ChatDisplayItem>().SetUp(channelName, senders, messages);
        }
        else
        {
            Instantiate(chatDisplayItemPrefab2, chatDisplayContent).GetComponent<ChatDisplayItem>().SetUp(channelName, senders, messages);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //Automatically show chat "on subscribed"
        //chatPanel.SetActive(true);
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    //NOTE: Start() will not be used
    //NOTE: Pwede 'to kapag mamaya, kasi may PhotonNetwork.Nickname naman
    //[SerializeField] string userID;//ginawang username
    // Start is called before the first frame update
    void Start()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();

            if (chatField.text != "" && Input.GetKey(KeyCode.Return))
            {
                SubmitPublicChatOnClick();
            }
        }

    }

    public void OnClickChatButton()
    {
        if (chatPanel.activeInHierarchy)
        {
            chatPanel.SetActive(false);
        }
        else chatPanel.SetActive(true);
    }
}
