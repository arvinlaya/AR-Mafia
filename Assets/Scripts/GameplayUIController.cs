using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{

    public GameObject chatBox;

    public void openChat()
    {
        if (chatBox != null)
        {
            if (chatBox.active == false)
            {
                chatBox.SetActive(true);
            }
            else
            {
                chatBox.SetActive(false);
            }
        }

    }
}
