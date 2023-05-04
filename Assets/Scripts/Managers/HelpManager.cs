using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour
{

    private enum PANEL_INDEX : int
    {
        STORY_INDEX,
        ROLES_INDEX,
        MECHANICS_INDEX,
        CREDITS_INDEX
    }

    public static HelpManager Instance;
    [SerializeField] private GameObject[] helpPanels;
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
    }

    public void openStoryPanel()
    {
        helpPanels[(int)PANEL_INDEX.STORY_INDEX].SetActive(true);
    }

    public void closeStoryPanel()
    {
        helpPanels[(int)PANEL_INDEX.STORY_INDEX].SetActive(false);
    }

    public void openRolesPanel()
    {
        helpPanels[(int)PANEL_INDEX.ROLES_INDEX].SetActive(true);
    }

    public void closeRolesPanel()
    {
        helpPanels[(int)PANEL_INDEX.ROLES_INDEX].SetActive(false);

    }

    public void openMechanicsPanel()
    {
        helpPanels[(int)PANEL_INDEX.MECHANICS_INDEX].SetActive(true);
    }

    public void closeMechanicsPanel()
    {
        helpPanels[(int)PANEL_INDEX.MECHANICS_INDEX].SetActive(false);
    }

    public void openCreditsPanel()
    {
        helpPanels[(int)PANEL_INDEX.CREDITS_INDEX].SetActive(true);
    }

    public void closeCreditsPanel()
    {
        helpPanels[(int)PANEL_INDEX.CREDITS_INDEX].SetActive(false);
    }
}
