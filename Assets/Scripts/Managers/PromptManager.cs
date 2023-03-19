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
            GameManager.Instance.rotateToCamera(ReferenceManager.Instance.prompt.transform, ReferenceManager.Instance.camera.transform);
        }
    }

    public IEnumerator promptTemporary(string message, float duration)
    {
        resetPrompt();

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(duration);

        Destroy(prompt);
        prompt = null;

        promptOn = false;
    }

    public IEnumerator promptStay(string message)
    {
        resetPrompt();

        prompt = Instantiate(ReferenceManager.Instance.prompt, new Vector3(0f, 6f, 0f), Quaternion.identity);
        promptOn = true;
        prompt.GetComponent<TextMeshPro>().text = message;

        yield return new WaitForSeconds(2f);
    }

    public IEnumerator promptAccuseVotes(Dictionary<Player, int> playerAccuseVotes, float duration)
    {
        resetPrompt();

        ReferenceManager.Instance.resultPrompt.SetActive(true);
        ResultPanel.Instance.resetPrompt();

        ResultPanel.Instance.setHeader("ACCUSATION RESULTS");
        ResultPanel.Instance.setBody("Player", "Votes");

        foreach (KeyValuePair<Player, int> player in playerAccuseVotes)
        {
            ResultPanel.Instance.addPlayer(player.Key.NickName, player.Value);
        }
        ReferenceManager.Instance.hideableCanvas.alpha = 1;

        yield return new WaitForSeconds(duration);

        ReferenceManager.Instance.hideableCanvas.alpha = 0;
        ResultPanel.Instance.resetPrompt();

        ReferenceManager.Instance.resultPrompt.SetActive(false);

        prompt = null;
        promptOn = false;


    }

    public IEnumerator promptEliminationVotes(int guiltyVotes, int innocentVotes, float duration)
    {
        resetPrompt();

        ReferenceManager.Instance.resultPrompt.SetActive(true);
        ResultPanel.Instance.resetPrompt();

        ResultPanel.Instance.setHeader("VOTE RESULTS");
        ResultPanel.Instance.setBody("GUILTY", "INNOCENT");
        ResultPanel.Instance.setBody($"\n{guiltyVotes}", $"{innocentVotes}");

        ReferenceManager.Instance.hideableCanvas.alpha = 1;
        ReferenceManager.Instance.resultPrompt.SetActive(true);

        yield return new WaitForSeconds(duration);

        ReferenceManager.Instance.hideableCanvas.alpha = 0;
        ResultPanel.Instance.resetPrompt();

        ReferenceManager.Instance.resultPrompt.SetActive(false);

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
