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
    [SerializeField] private TMP_Text promptHeader;
    [SerializeField] private CanvasGroup promptHeaderCanvasGroup;
    [SerializeField] private GameObject alertPrompt;
    [SerializeField] private GameObject preRoleVillager;
    [SerializeField] private GameObject preRoleMafia;
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

    public IEnumerator revealPreRoleVillager()
    {
        preRoleVillager.SetActive(true);

        yield return new WaitForSeconds(7f);

        preRoleVillager.SetActive(false);
    }
    public IEnumerator revealPreRoleMafia()
    {
        preRoleMafia.SetActive(true);

        yield return new WaitForSeconds(7f);

        preRoleMafia.SetActive(false);
    }
    public IEnumerator promptRoleMessage(string role, int mafiaCount)
    {

        string message = "";

        switch (role)
        {
            case "VILLAGER":
                message = "You are a villager. Be vigilant and stay safe.";
                break;

            case "MAFIA":
                if (mafiaCount > 1)
                {
                    message = "Who would you like to kill tonight?\nCommunicate with your co-mafia and select a player to murder.";
                }
                else
                {
                    message = "Who would you like to kill tonight? Select a player to murder.";
                }
                break;

            case "DOCTOR":
                message = "You're the <b><color=\"blue\">Doctor</color></b>. Select a player you want to save.";
                break;

            case "DETECTIVE":
                message = "You're the <b><color=\"yellow\">Detective</color></b>. Select a player you want to investigate.";
                break;
        }

        this.promptHeader.SetText(message);
        promptHeaderCanvasGroup.alpha = 1;

        yield return null;
    }
    public IEnumerator promptWithDelay(string message, float delay)
    {
        this.promptHeader.SetText(message);
        promptHeaderCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(delay);
        promptHeaderCanvasGroup.alpha = 0;
    }
    public IEnumerator promptNoDelay(string message)
    {
        this.promptHeader.SetText(message);
        promptHeaderCanvasGroup.alpha = 1;

        yield return null;
    }

    public IEnumerator alertPromptWithDelay(string message, float delay)
    {
        alertPrompt.SetActive(true);
        alertPrompt.GetComponentInChildren<TMP_Text>().SetText(message);

        yield return new WaitForSeconds(delay);

        alertPrompt.SetActive(false);
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

    public IEnumerator actionReveal(string role, string targetName, Transform houseLocation)
    {
        string message = "";
        Vector3 offset = new Vector3(-1, 5, 0);

        GameObject roleRevealPrefab = Instantiate(ReferenceManager.Instance.RoleRevealPrefab, houseLocation);
        roleRevealPrefab.transform.localPosition += offset;

        if (role.Trim() == "MAFIA")
        {
            message = "<color=\"red\">Killing</color> " + targetName;
        }
        else if (role.Trim() == "DOCTOR")
        {
            message = "<color=\"blue\">Saving</color> " + targetName;
        }

        roleRevealPrefab.GetComponent<TextMesh>().text = message;

        yield return new WaitForSeconds(2f);

        Destroy(roleRevealPrefab);
    }

    public void callRoleReveal(string role, Transform houseLocation)
    {
        StartCoroutine(roleReveal(role, houseLocation));
    }
    public void callActionReveal(string role, string targetName, Transform houseLocation)
    {
        StartCoroutine(actionReveal(role, targetName, houseLocation));
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
