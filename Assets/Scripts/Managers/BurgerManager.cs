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
