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
    private Animator storyPanelAnimator;
    private Animator rolesPanelAnimator;
    private Animator mechanicsPanelAnimator;
    private Animator creditsPanelAnimator;
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
        storyPanelAnimator = helpPanels[(int)PANEL_INDEX.STORY_INDEX].GetComponent<Animator>();
        rolesPanelAnimator = helpPanels[(int)PANEL_INDEX.ROLES_INDEX].GetComponent<Animator>();
        mechanicsPanelAnimator = helpPanels[(int)PANEL_INDEX.MECHANICS_INDEX].GetComponent<Animator>();
        creditsPanelAnimator = helpPanels[(int)PANEL_INDEX.CREDITS_INDEX].GetComponent<Animator>();
    }

    public void openStoryPanel()
    {
        storyPanelAnimator.SetBool("isOpen", true);
    }

    public void closeStoryPanel()
    {
        storyPanelAnimator.SetBool("isOpen", false);
    }

    public void openRolesPanel()
    {
        rolesPanelAnimator.SetBool("isOpen", true);
    }

    public void closeRolesPanel()
    {
        rolesPanelAnimator.SetBool("isOpen", false);
    }

    public void openMechanicsPanel()
    {
        mechanicsPanelAnimator.SetBool("isOpen", true);
    }

    public void closeMechanicsPanel()
    {
        mechanicsPanelAnimator.SetBool("isOpen", false);
    }

    public void openCreditsPanel()
    {
        creditsPanelAnimator.SetBool("isOpen", true);
    }

    public void closeCreditsPanel()
    {
        creditsPanelAnimator.SetBool("isOpen", false);
    }
}
