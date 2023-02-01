using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatDisplayItem : MonoBehaviour
{
    //TODO: Yung Avatar?
    [SerializeField] TMP_Text text;

    public void SetUp(string channelName, string[] senders, object[] messages)
    {

        if (messages[0].ToString().Contains("(MAFIA)"))
        {
            text.color = Color.red;
            text.text = string.Format("{0}{2}: {1}", senders[0], messages[0], "(Mafia)");
        }
        else
        {
            //channel = ROOM from PUN2
            text.color = Color.white;
            text.text = string.Format("{0}: {1}", senders[0], messages[0], "(Mafia)");
        }
    }
}
