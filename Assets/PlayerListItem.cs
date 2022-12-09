using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerIGN;
    [SerializeField] TMP_Text playerPosition;
    [SerializeField] GameObject hostLabel;
    [SerializeField] GameObject kickButton;
    Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        playerIGN.text = _player.NickName;
        playerPosition.text = "Player " + _player.ActorNumber.ToString();
        //if (PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.NickName )
        if (PhotonNetwork.MasterClient.NickName == _player.NickName)
        {
            hostLabel.SetActive(true);
            kickButton.SetActive(false);
        }
        //else hostLabel.SetActive(false);//GOOD for "Host", Problem: Client should not see "kick" button
        else if(PhotonNetwork.LocalPlayer.NickName != PhotonNetwork.MasterClient.NickName)
        {
           kickButton.SetActive(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
