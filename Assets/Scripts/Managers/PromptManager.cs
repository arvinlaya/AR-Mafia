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
    private GameObject prompt;
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
        resetPrompt();

        string message = player.NickName + " was poisoned after they had a conversation with the mafia last night.";

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);
        prompt = null;

        promptOn = false;
    }
    public IEnumerator promptDayDiscussionPhase(Player player)
    {
        resetPrompt();

        string message = "The village woke up and will start discussing about the event that occured last night.";

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(2f);
    }
    public IEnumerator promptAccused(Player player)
    {
        resetPrompt();

        string message = player.NickName + " is accused as a murderer by the village.";

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);
        prompt = null;

        promptOn = false;
    }

    public IEnumerator promptAccusedPhase(Player player)
    {
        resetPrompt();

        string message = "The village will now vote if " + player.NickName + "is guilty or innocent.";

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(2f);
    }

    public IEnumerator promptGuilty(Player player)
    {
        resetPrompt();

        string message = "The village agreed that " + player.NickName + " is the murderer.";

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(5f);

        Destroy(prompt);
        prompt = null;

        promptOn = false;
    }

    public void resetPrompt()
    {
        if (promptOn == true)
        {
            Destroy(prompt);
        }
        promptOn = false;
    }
}
