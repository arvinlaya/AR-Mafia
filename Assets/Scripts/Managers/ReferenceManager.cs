using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ReferenceManager : MonoBehaviour
{

    [SerializeField] public TMP_Text UITimer;
    [SerializeField] public Transform middle;
    [SerializeField] public GameObject panelParent;
    [SerializeField] public GameObject RoleRevealPrefab;
    [SerializeField] public GameObject[] villagerPanels;
    [SerializeField] public GameObject[] doctorPanels;
    [SerializeField] public GameObject[] mafiaPanels;
    [SerializeField] public GameObject[] detectivePanels;
    [SerializeField] public GameObject myRoleVillager;
    [SerializeField] public GameObject myRoleDoctor;
    [SerializeField] public GameObject myRoleMafia;
    [SerializeField] public GameObject myRoleDetective;
    [SerializeField] public GameObject prompt;
    [SerializeField] public GameObject camera;
    [SerializeField] public Material[] ButtonMaterials;
    [SerializeField] public GameObject[] Models;
    [SerializeField] public GameObject victoryPromptParent;
    [SerializeField] public GameObject victoryVillagerPrompt;
    [SerializeField] public GameObject victoryMafiaPrompt;
    [SerializeField] public GameObject resultPrompt;
    [SerializeField] public CanvasGroup hideableCanvas;
    [SerializeField] public CanvasGroup hideableUI;
    public int time;
    public int LayerIgnoreRaycast;
    public int LayerHouse;
    public static ReferenceManager Instance;

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
        LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        LayerHouse = LayerMask.NameToLayer("House");
    }
}
