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
    [SerializeField] GameObject promtFull;
    [SerializeField] GameObject privateCodeModal;
    [SerializeField] TMP_InputField privateCodeInputField;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = "Room " + _info.Name;
        //Debug.Log("inside Room LIST ITEM: CUSTOM PROP::: "+ _info.CustomProperties["RoomOwner"]);
        //text.text = _info.CustomProperties["RoomOwner"] + "'s Public Room";
        player_Count.text = _info.PlayerCount + "/8";


        if(info.CustomProperties.ContainsKey("isPrivate"))
        {

        string correctKey = (info.CustomProperties["room_code"].ToString());
        Debug.LogError("THE KEY(custom prop String:::" + correctKey);
        }


        // _info contains what we NEED






    }

    public void OnClick()
    {
        //IMPLEMENT CHECK IF PRIVATE
        //SHOW Enter CODE Modal on true

        // ! INFO is EMPTY
        if (!info.IsOpen)
        {
            promtFull.gameObject.SetActive(true);
        }
        else if (info.CustomProperties.ContainsKey("isPrivate") &&
           info.CustomProperties.ContainsKey("room_code")) //is a PR
        {

        string correctKey = (info.CustomProperties["room_code"].ToString());
            Debug.Log("Custom Prop room_code: " + correctKey);
            //IF Private, only show Private modal
            privateCodeModal.SetActive(true);
        }
        else //Public, go lang...
        {
            
            Debug.Log("PUBLIC CLICK");
            Launcher.Instance.JoinRoom(info);
        }
    }


    //OnClickConfirm private
    public void OnClickEnterPrivate()
    {
        string correctKey = (info.CustomProperties["room_code"].ToString());
        string userInput = privateCodeInputField.text;

        Debug.Log("RoomListItem Enter Private: ");
        Debug.Log(correctKey + "," + userInput);
        if (correctKey == userInput)
        {
            Launcher.Instance.JoinRoom(info);
            Launcher.Instance.JoinRoomPrivate();
            MenuManager.Instance.OpenMenu("loading");
        }
        else
            promtFull.gameObject.SetActive(true);
    }

}
