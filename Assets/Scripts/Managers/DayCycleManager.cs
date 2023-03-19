using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public static DayCycleManager Instance;
    public enum DAY_STATE : byte
    {
        DAY,
        NIGHT
    }

    [SerializeField]
    Animator dayCycleAnimator;

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

    public void setDayState(DAY_STATE dayState)
    {
        switch (dayState)
        {
            case DAY_STATE.NIGHT:
                dayCycleAnimator.SetBool("IsDay", false);
                dayCycleAnimator.SetBool("IsNight", true);
                break;

            case DAY_STATE.DAY:
                dayCycleAnimator.SetBool("IsNight", false);
                dayCycleAnimator.SetBool("IsDay", true);
                break;

            default:
                Debug.LogError("WRONG DAY STATE VALUE");
                break;
        }
    }



}
