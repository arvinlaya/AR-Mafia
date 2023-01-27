using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public float timeLeft = 5.0f;
    [SerializeField] TMP_Text countDownTMP;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        // Convert integer to string
        countDownTMP.text = "Game begins in " + (timeLeft).ToString("0") + "...";
        if (timeLeft < 1)
        {
            countDownTMP.text = "GO";
        }
    }
}
