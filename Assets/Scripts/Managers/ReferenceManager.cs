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
    public int time;
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
}
