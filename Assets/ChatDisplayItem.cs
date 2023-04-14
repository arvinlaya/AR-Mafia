using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatDisplayItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text senderIGN;
    [SerializeField] GameObject chatBubble;

    public void SetUp(string channelName, string[] senders, object[] messages)
    {

        if (messages[0].ToString().Contains("(MAFIA)"))
        {
            senderIGN.color = Color.red;
            senderIGN.text = string.Format(senders[0]);
            text.text = string.Format("{0}", messages[0].ToString().Replace("(MAFIA)",""));

        }
        else
        {
            //channel = ROOM from PUN2
            senderIGN.color = Color.black;
            //text.color = Color.white;

            senderIGN.text = string.Format(senders[0]);
            text.text = string.Format("{0}", messages[0], "(Mafia)");
        }
    }
}
