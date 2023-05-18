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

public class voiceTestManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Recorder recorder;

    private void Start()
    {
        RequestRecordAudioPermission();
    }

    private void RequestRecordAudioPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Get the current activity
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Request the RECORD_AUDIO permission
        string permission = "android.permission.RECORD_AUDIO";
        currentActivity.Call("requestPermissions", new string[] { permission }, 0);
#endif
    }
        
    public void OnClickRecorderTransmit()
    {
        recorder.TransmitEnabled = !recorder.TransmitEnabled;
    }

}
