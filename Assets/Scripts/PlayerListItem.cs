using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerIGN;
    [SerializeField] TMP_Text playerPosition;
    [SerializeField] GameObject hostLabel;
    [SerializeField] GameObject kickButton;
    Player player;//LOCAL PLAYER INFO
    [SerializeField] TMP_Text kickPlayerIGN;

    public RoomInfo info; //LOCAL ROOM INFO

    public void SetUp(Player _player)
    {
        player = _player;
        playerIGN.text = _player.NickName;
        kickPlayerIGN.text = _player.NickName;
        playerPosition.text =  _player.ActorNumber.ToString();

        // room master name = this player item card
        if (PhotonNetwork.MasterClient.NickName == _player.NickName)
        {
            hostLabel.SetActive(true);
            kickButton.SetActive(false);
        }
        else if(player.NickName != PhotonNetwork.MasterClient.NickName) // This player is not the room master
        {
            if (PhotonNetwork.IsMasterClient)
            {
                hostLabel.SetActive(false);
                kickButton.SetActive(true);
            }
            else//not room master 
            {
                hostLabel.SetActive(false);
                kickButton.SetActive(false);
            }
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

    public void OnClickKickButton()
    {
        Launcher.Instance.KickPlayer(player);
    }
}
