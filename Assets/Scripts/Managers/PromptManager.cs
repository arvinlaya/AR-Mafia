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
    [SerializeField]
    private TMP_Text promptHeader;
    [SerializeField]
    private CanvasGroup promptHeaderCanvasGroup;
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

    public IEnumerator promptWithDelay(string message, float delay)
    {
        promptHeaderCanvasGroup.alpha = 1;
        this.promptHeader.SetText(message);

        yield return new WaitForSeconds(delay);
        promptHeaderCanvasGroup.alpha = 0;
    }
    public IEnumerator promptNoDelay(string message)
    {
        promptHeaderCanvasGroup.alpha = 1;
        this.promptHeader.SetText(message);

        yield return null;
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

    public IEnumerator roleReveal(string role, Transform houseLocation)
    {
        Vector3 offset = new Vector3(-1, 5, 0);

        GameObject roleRevealPrefab = Instantiate(ReferenceManager.Instance.RoleRevealPrefab, houseLocation);
        roleRevealPrefab.transform.localPosition += offset;

        if (role.Trim() == "MAFIA")
        {
            role = "<color=\"red\">" + role + "</color>";
        }
        else if (role.Trim() == "DOCTOR")
        {
            role = "<color=\"blue\">" + role + "</color>";
        }
        else
        {
            role = "<color=\"yellow\">" + role + "</color>";
        }

        roleRevealPrefab.GetComponent<TextMesh>().text = role;

        yield return new WaitForSeconds(2f);

        Destroy(roleRevealPrefab);
    }

    public void callRoleReveal(string role, Transform houseLocation)
    {
        StartCoroutine(roleReveal(role, houseLocation));
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
