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

    private bool isMuted = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
        speaker = GetComponent<Speaker>();

        ///gm = FindObjectOfType<GameManager>();

        audioSource = GetComponent<AudioSource>();

    }

    //void someFunc(GameManager.Instance)
    
    // Update is called once per frame
    void Update()
    {

        //Debug.LogError("Current Phase:" + gm.currentPhase);

        //if(gm != null)
        //{
        //    if (gm.currentPhase == GameManager.GAME_PHASE.NIGHT)
        //    {
        //        if (PhotonNetwork.IsConnected && photonView.IsMine)
        //        {
        //            // Mute the speaker
        //            audioSource.volume = 0;
        //            isMuted = true;
        //        }
        //    }
        //    else
        //    {
        //        // Unmute the speaker
        //        audioSource.volume = 1;
        //        isMuted = false;
        //    }

        //}

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
