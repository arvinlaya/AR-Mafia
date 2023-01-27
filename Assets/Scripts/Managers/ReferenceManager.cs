using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReferenceManager : MonoBehaviour
{

    [SerializeField] public TMP_Text UITimer;
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
