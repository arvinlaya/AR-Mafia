using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text text;
	[SerializeField] TMP_Text player_Count;

	public RoomInfo info;

	public void SetUp(RoomInfo _info)
	{
		info = _info;
		text.text = _info.Name;
		player_Count.text = _info.PlayerCount + "/8";
	}

	public void OnClick()
	{
		Launcher.Instance.JoinRoom(info);
	}
}
