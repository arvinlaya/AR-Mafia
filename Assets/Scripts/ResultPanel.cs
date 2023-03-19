using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] public TMP_Text header;
    [SerializeField] public TMP_Text left;
    [SerializeField] public TMP_Text right;

    public static ResultPanel Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void addPlayer(string playerName, int playerVotes)
    {
        string playerMessage = $"\n{playerName}";
        string voteMessage = $"\n{playerVotes}";

        left.SetText(left.text + playerMessage);
        right.SetText(right.text + voteMessage);
    }

    public void setHeader(string text)
    {
        header.SetText(text);
    }

    public void setBody(string leftText, string rightText)
    {
        left.SetText(left.text + leftText);
        right.SetText(right.text + rightText);
    }

    public void resetPrompt()
    {
        header.SetText("");
        left.SetText("");
        right.SetText("");
    }

}
