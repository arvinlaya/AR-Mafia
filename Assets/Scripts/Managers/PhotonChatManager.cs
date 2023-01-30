//ERR: Null Exception, chatField object

using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
//using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] TMP_Text chatDisplay;
    Player player;

    private string myChannelName;

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
            if (currentChat.Contains("/mafia"))
            {
                chatClient.PublishMessage("MafiaCH", currentChat);
            }
            else
            {
            //channel = ROOM from PUN2
            chatClient.PublishMessage(myChannelName, currentChat);
            }


            chatField.text = "";
            //currentChat = "sample: " + DateTime.Now.ToString();
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
        isConnected = true;
        //joinChatbutton.SetActive(false);
        //use room name as channel name?
        myChannelName = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(new string[] { myChannelName });
        //TODO: Kapag okay na yung sa actual game
        //if (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"] == "MAFIA")
        //{
        //    chatClient.Subscribe(new string[] { "MafiaCH" });
        //}

        //hardcode
        if (PhotonNetwork.LocalPlayer.NickName == "Mafia1" ||PhotonNetwork.LocalPlayer.NickName == "Mafia2" )
        {
            chatClient.Subscribe(new string[] { "MafiaCH" });
        }


        Debug.Log("Channel name is" + myChannelName);
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);

            chatDisplay.text += "\n " + msgs;

            Debug.Log(msgs);

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
        Debug.Log("hello, I'm: " + PhotonNetwork.NickName);//username for CHAT
        Debug.Log("Room/Channel: " + PhotonNetwork.MasterClient.NickName + "'s channel");
        Debug.Log("Room Owner Player(for channel name only)" + PhotonNetwork.MasterClient.NickName);
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
        }

        //if (chatField.text != "" && Input.GetKey(KeyCode.Return))
        //{
        //    SubmitPublicChatOnClick();
        //}

    }

    public void OnClickChatButton()
    {
        if (chatPanel.activeInHierarchy)
        {
            chatPanel.SetActive(false);
        } else chatPanel.SetActive(true);
    }
}
