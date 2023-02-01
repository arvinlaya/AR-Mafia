using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.IO;
public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance;
    private bool promptOn;
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void FixedUpdate()
    {
        if (promptOn)
        {
            GameManager.Instance.rotateToCamera(ReferenceManager.Instance.prompt, ReferenceManager.Instance.camera);
        }
    }


    public IEnumerator promptMurdered(Player player)
    {
        string message = player.NickName + " was poisoned after they had a conversation with the mafia last night.";
        GameObject prompt = null;

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);

        promptOn = false;
    }
    public IEnumerator promptDayDiscussion(Player player)
    {
        string message = "The village woke up and will start discussing about the event that occured last night.";
        GameObject prompt = null;

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);

        promptOn = false;
    }
    public IEnumerator promptAccused(Player player)
    {
        string message = player.NickName + " is accused as a murderer by the village.";
        GameObject prompt = null;

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);

        promptOn = false;
    }

    public IEnumerator promptGuilty(Player player)
    {
        string message = "The village agreed that " + player.NickName + " is the murderer.";
        GameObject prompt = null;

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);

        promptOn = false;
    }
}
