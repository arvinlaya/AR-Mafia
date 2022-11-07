using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneMg : MonoBehaviour
{
    [SerializeField] GameObject exitModal;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                if (exitModal)
                {
                    exitModal.SetActive(true);
                }
            }
        }
    }

    public void onUserClickYesNo(int choice)
    {//choice==0 no     choice==1 yes
        if (choice == 1)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
            Debug.Log("App should already be quit");
        }
        exitModal.SetActive(false);//else
    }

    public void GoToHome(){
        SceneManager.LoadScene("Home");
    }
    public void Stay(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        exitModal.SetActive(false);//else
    }
    public void GoToOnlineRooms(){
        SceneManager.LoadScene("OnlineRooms");
    }
}