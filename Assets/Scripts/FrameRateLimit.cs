using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimit : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
