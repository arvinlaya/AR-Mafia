using Photon.Realtime;
using UnityEngine;

class Mafia : Role
{
    public string ROLE_TYPE { get; set; }


    public Mafia()
    {
        ROLE_TYPE = "MAFIA";
    }
    public void skill(Player target)
    {
        target.CustomProperties["IS_DEAD"] = true;
    }
}