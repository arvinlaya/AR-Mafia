using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class BurgerManager : MonoBehaviour
{
    public static BurgerManager Instance;
    private Animator burgerAnimator;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        burgerAnimator = GetComponent<Animator>();
    }
    public void openBurgerPanel()
    {
        burgerAnimator.SetBool("isOpen", true);
    }

    public void closeBurgerPanel()
    {
        burgerAnimator.SetBool("isOpen", false);
    }

    public void myRole()
    {

    }

    public void rules()
    {

    }
    public void exit()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= GameManager.Instance.OnEvent;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("REMOVED");
            SceneManager.sceneLoaded -= GameManager.Instance.OnSceneLoad;
        }

        Destroy(RoomManager.Instance.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
