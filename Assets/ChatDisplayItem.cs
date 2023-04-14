using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatDisplayItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text senderIGN;
    [SerializeField] GameObject bubble;

    public void SetUp(string channelName, string[] senders, object[] messages)
    {

        if (messages[0].ToString().Contains("(MAFIA)"))
        {
            text.color = Color.red;
            //sample:
            //Player1(Mafia):Message Here
            text.text = string.Format("{0}{1}", senders[0], messages[0].ToString().Replace("(MAFIA)","(Mafia):"));
        }
        else
        {
            //channel = ROOM from PUN2
            text.color = Color.white;
            text.text = string.Format("{0}: {1}", senders[0], messages[0], "(Mafia)");
        }
    }
}
