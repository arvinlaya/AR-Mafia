using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class ConnectionStateManager : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject reconModal;
    public int countdownSeconds = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("Disconnected from server. Attempting to reconnect...");
        //ADD RECONNECT PROPERTY on the player
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps.Add("Reconnect", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        // Attempt to reconnect
        StartCoroutine(Reconnect());

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

            // Provide feedback to the player
            // You can add code here to display a loading spinner or a message to inform the player that the game is attempting to reconnect.

            //reconModal.SetActive(true);
        }

    }

    IEnumerator Reconnect()
    {
        while (PhotonNetwork.IsConnected == false)
        {
            Debug.LogError("Trying to reconnect");

            yield return new WaitForSeconds(1);

            PhotonNetwork.ReconnectAndRejoin();

            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //WARNING:
                //Problem??? nag ra-run pa rin kahit na 'di pa connected?

                // Reconnection successful
                Debug.Log("PhotonNetwork.IsConnected == TRUE ???");
                // Inform the player and resume the game
                // You can add code here to inform the player that the game has reconnected and to resume the game.

                reconModal.SetActive(false);
            }
            else
            {
                // Reconnection failed
                Debug.LogError("Reconnection failed.");
                // You can add code here to inform the player that the game failed to reconnect and to provide the option to try again.
            }
        }
    }

    //NOTE: not called on HOME SCENE
    public override void OnJoinedRoom()
    {
        Debug.Log("You joined a room:" + PhotonNetwork.CurrentRoom.Name + "...\n");
        // check if this is a reconnection
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Reconnect"))
        {
            // re-instantiate the player object
            //InstantiatePlayer();
            Debug.LogError("RECONNECTION Success");

            // remove the reconnect flag
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Reconnect");
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == PhotonNetwork.LocalPlayer)
        {
            // The local player has left the room
            Debug.LogError("Disconnected. Trying to reconnect...");
            // Notify other players that the local player is trying to reconnect
            photonView.RPC("NotifyPlayerDisconnected", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            reconModal.SetActive(true);
        }
    }

    private async void StartPlayerTimeOutCountdown()
    {
        await CountdownAsync(countdownSeconds);
        // Code to execute after countdown is finished
        Debug.Log("Countdown finished!");
        reconModal.SetActive(false);
    }

    private async Task CountdownAsync(int seconds)
    {
        while (seconds > 0)
        {
            Debug.Log(seconds);
            await Task.Delay(1000);
            seconds--;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer == PhotonNetwork.LocalPlayer)
        {
            // The local player has joined the room (either for the first time or after a disconnection)
            Debug.Log("Connected!");
            // Restore the player's state if it was saved before
            photonView.RPC("RestorePlayerState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            // Another player has joined the room
            reconModal.SetActive(false);
        }
    }

}

