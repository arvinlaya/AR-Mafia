// * This handles Room search/filter scripts
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab;

    public Transform roomListContent;
    public TMP_InputField searchInputField;

    public Transform roomListContent_Private;
    public TMP_InputField searchInputField_Private;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        //UpdateRoomListUI();
    }

    public void UpdateRoomListUI()
    {
        if (this.roomList.Count <= 0)
        {
            Debug.Log("Empty Room");
        }
        else
        {
            foreach (Transform child in roomListContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in roomListContent_Private.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (RoomInfo roomInfo in roomList)
            {
                // Filter rooms based on room name
                if (!string.IsNullOrEmpty(searchInputField.text)
                    && !roomInfo.Name.ToLower().Contains(searchInputField.text.ToLower()))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(searchInputField_Private.text)
                    && !roomInfo.Name.ToLower().Contains(searchInputField_Private.text.ToLower()))
                {
                    continue;
                }

                if (roomInfo.RemovedFromList)
                    continue; // ignore/don't display

                else if (roomInfo.CustomProperties.ContainsKey("isPrivate"))//is Private, different component
                {
                    Instantiate(roomListItemPrefab, roomListContent_Private).GetComponent<RoomListItem>().SetUp(roomInfo);
                }
                else
                    Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);

            }

        }
    }
}

