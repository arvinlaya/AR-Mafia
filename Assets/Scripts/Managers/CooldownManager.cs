using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private int doorCastTime;
    private bool isDoorCooldown;
    private bool isSkillCooldown;
    public static CooldownManager Instance;

    public static int DOOR_COOLDOWN = 15; //15

    // ABILITY IS ONE TIME USE PER NIGHT.

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool getIsDoorCooldown()
    {
        return isDoorCooldown;
    }
    public bool getIsSkillCooldown()
    {
        return isSkillCooldown;
    }
    public void setDoorCooldown(bool state)
    {
        isDoorCooldown = state;
    }
    public void setSkillCooldown(bool state)
    {
        isSkillCooldown = state;
    }

    public void setDoorCastTime(int time)
    {
        doorCastTime = time;
    }

    public void doorCooldownCheck(int time)
    {
        if ((doorCastTime - DOOR_COOLDOWN) > time)
        {
            setDoorCooldown(false);
        }
    }

    public int getDoorCooldownRemaining(int time)
    {
        return time - (doorCastTime - DOOR_COOLDOWN);
    }

}
