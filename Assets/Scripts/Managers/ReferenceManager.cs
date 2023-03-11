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
    [SerializeField] public GameObject[] rolePanels;
    [SerializeField] public GameObject prompt;
    [SerializeField] public GameObject camera;
    [SerializeField] public Material[] ButtonMaterials;
    [SerializeField] public GameObject[] Models;
    [SerializeField] public GameObject eliminationVotePrompt;
    [SerializeField] public GameObject lighting;
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
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        LayerHouse = LayerMask.NameToLayer("House");
    }
}
