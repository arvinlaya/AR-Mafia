using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class BurgerManager : MonoBehaviour
{
    public static BurgerManager Instance;
    [SerializeField] private GameObject rulesPanel;
    private Animator burgerAnimator;
    private Animator rulesAnimator;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        burgerAnimator = GetComponent<Animator>();
        rulesAnimator = rulesPanel.gameObject.GetComponent<Animator>();
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

    public void closeMyRole()
    {
    }

    public void openRules()
    {
        rulesAnimator.SetBool("isOpen", true);
    }

    public void closeRules()
    {
        rulesAnimator.SetBool("isOpen", false);
    }
    public void exit()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= GameManager.Instance.OnEvent; // important

        if (PhotonNetwork.IsMasterClient) // important
        {
            Debug.Log("REMOVED");
            SceneManager.sceneLoaded -= GameManager.Instance.OnSceneLoad; //important
            PhotonNetwork.NetworkingClient.EventReceived -= GameManager.Instance.OnEvent; //important
        }

        Destroy(RoomManager.Instance.gameObject); // important
        StartCoroutine(DisconnectAndLoad()); // important
    }

    IEnumerator DisconnectAndLoad() // change content of this function
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
