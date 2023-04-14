using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class BurgerManager : MonoBehaviour
{
    public static BurgerManager Instance;
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private GameObject myRolePanel;
    private Animator burgerAnimator;
    private Animator rulesAnimator;
    private Animator myRoleAnimator;
    private bool isActivated;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        isActivated = false;
        burgerAnimator = GetComponent<Animator>();
        rulesAnimator = rulesPanel.gameObject.GetComponent<Animator>();
        myRoleAnimator = myRolePanel.gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        switch ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
        {
            case "VILLAGER":
                ReferenceManager.Instance.myRoleVillager.SetActive(true);
                break;

            case "DOCTOR":
                ReferenceManager.Instance.myRoleDoctor.SetActive(true);
                break;

            case "MAFIA":
                ReferenceManager.Instance.myRoleMafia.SetActive(true);
                break;

            case "DETECTIVE":
                ReferenceManager.Instance.myRoleDetective.SetActive(true);
                break;
        }
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
        if (!isActivated)
        {
            switch ((string)PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
            {
                case "VILLAGER":
                    ReferenceManager.Instance.myRoleVillager.SetActive(true);
                    break;

                case "DOCTOR":
                    ReferenceManager.Instance.myRoleDoctor.SetActive(true);
                    break;

                case "MAFIA":
                    ReferenceManager.Instance.myRoleMafia.SetActive(true);
                    break;

                case "DETECTIVE":
                    ReferenceManager.Instance.myRoleDetective.SetActive(true);
                    break;
            }
            isActivated = true;
        }
        myRoleAnimator.SetBool("isOpen", true);
    }

    public void closeMyRole()
    {
        myRoleAnimator.SetBool("isOpen", false);
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
        //TODO: PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
