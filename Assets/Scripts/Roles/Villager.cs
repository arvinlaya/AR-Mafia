using Photon.Realtime;
using UnityEngine;

class Villager : Role
{
    public string ROLE_TYPE { get; set; }


    public Villager()
    {
        ROLE_TYPE = "VILLAGER";
    }
    public void skill(Player target)
    {
        Debug.Log("JUST A VILLAGER");
    }
}