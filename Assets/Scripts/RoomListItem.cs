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

	public RoomInfo info;

	public void SetUp(RoomInfo _info)
	{
		info = _info;
		text.text = "Room " + _info.Name;
		//Debug.Log("inside Room LIST ITEM: CUSTOM PROP::: "+ _info.CustomProperties["RoomOwner"]);
		//text.text = _info.CustomProperties["RoomOwner"] + "'s Public Room";
		player_Count.text = _info.PlayerCount + "/8";
	}

	public void OnClick()
	{
        if (!info.IsOpen)
        {
			promtFull.gameObject.SetActive(true);
        }else
		Launcher.Instance.JoinRoom(info);
	}
}
