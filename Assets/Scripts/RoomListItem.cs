using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text player_Count;
    [SerializeField] TMP_Text promptText;
    [SerializeField] GameObject promtFull;
    [SerializeField] GameObject privateCodeModal;
    [SerializeField] TMP_InputField privateCodeInputField;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
        player_Count.text = _info.PlayerCount + "/"+ Launcher.Instance.getMaxPlayer();
    }

    public void OnClick()
    {
        if (!info.IsOpen)
            promtFull.gameObject.SetActive(true);
        else if (info.CustomProperties.ContainsKey("isPrivate")) //is a PR
        {
            string correctKey = (string)info.CustomProperties["password"];
            Debug.Log("Custom Prop INSIDE ONCLICK: " + correctKey);
            privateCodeModal.SetActive(true);
        }
        else //Public
            Launcher.Instance.JoinRoom(info);
    }


    public void OnClickOkayPrompt()
    {
        promptText.text = "Room is FULL!";
        privateCodeInputField.text = "";
    }

    //OnClickConfirm private
    public void OnClickEnterPrivate()
    {
        string correctKey = (string)info.CustomProperties["password"];
        string userInput = privateCodeInputField.text.ToLower();

        Debug.Log("RoomListItem Enter Private: ");
        Debug.Log("Match?: " + correctKey + "," + userInput);
        if (correctKey == userInput)
        {
            Launcher.Instance.JoinRoom(info);
            Launcher.Instance.JoinRoomPrivate();
            MenuManager.Instance.OpenMenu("loading");
        }
        else //TODO change Full to "WRONG PASSWORD
        {
            promptText.text = "Wrong Password!";
            promtFull.gameObject.SetActive(true);
        }
    }

}
