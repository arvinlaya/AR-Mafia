using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KickPlayerScript :  MonoBehaviourPunCallbacks
{
    Player player;
    [SerializeField] TMP_Text playerIGN;

    public void SetPlayerInfoForKicking(Player _kickThisPlayer)
    {
        Debug.LogError("Kicking," + _kickThisPlayer.NickName);
        player = _kickThisPlayer;
        playerIGN.text = _kickThisPlayer.NickName;
    }

    public void OnClickKickButtonConfirm()
    {
        Debug.LogError("kick: " + player.NickName);
        Launcher.Instance.KickPlayer(player);
    }
    
}
