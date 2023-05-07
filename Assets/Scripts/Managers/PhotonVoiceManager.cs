using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice;
using Photon.Voice.Unity;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonVoiceManager : MonoBehaviourPunCallbacks
{

    private Recorder recorder;
    private Speaker speaker;
    private VoiceConnection vc;

    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
        speaker = GetComponent<Speaker>();

        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.LogError("Current Phase:" + gm.currentPhase);
        if (gm != null)
        {
            switch (gm.currentPhase)
            {
                case GameManager.GAME_PHASE.NIGHT:
                    if (PhotonNetwork.IsConnected && photonView.IsMine)
                    {
                        photonView.RPC("StopAllRecording", RpcTarget.All);
                    }
                    break;
                case GameManager.GAME_PHASE.DAY_DISCUSSION:
                    if (PhotonNetwork.IsConnected && photonView.IsMine)
                    {
                        photonView.RPC("EnableAllRecording", RpcTarget.All);
                    }
                    break;
                default:
                    break;

            }
        }

    }

    [PunRPC]
    private void EnableAllRecording()
    {
        recorder.TransmitEnabled = true;
    }


    [PunRPC]
    private void StopAllRecording()
    {
        recorder.TransmitEnabled = false;
    }

}
