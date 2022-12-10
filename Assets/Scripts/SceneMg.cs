//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//
//
//public class SceneMg : MonoBehaviour
//{
//    [SerializeField] GameObject exitDialog;
//    [SerializeField] GameObject newGameDialog;
//    [SerializeField] GameObject joinGameDialog;
//
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (SceneManager.GetActiveScene().buildIndex != 0)
//            {
//                SceneManager.LoadScene(0);
//            }
//            else
//            {
//                if (exitModal)
//                {
//                    exitModal.SetActive(true);
//                }
//            }
//        }
//    }

//    public void onUserClickYesNo(int choice)
//    {//choice==0 no     choice==1 yes
//        if (choice == 1)
//        {
//            UnityEditor.EditorApplication.isPlaying = false;
//            Application.Quit();
//            Debug.Log("App should already be quit");
//        }
//        exitDialog.SetActive(false);//else
//    }
//
//    public void Stay(){
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        exitDialog.SetActive(false);//else
//    }
//    public void GoToOnlineRooms(){
//        SceneManager.LoadScene("OnlineRooms");
//    }
//}