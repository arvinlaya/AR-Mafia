using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
class Mafia : Role
{
    public string ROLE_TYPE { get; set; }


    public Mafia()
    {
        ROLE_TYPE = "MAFIA";
    }
    public void skill(Player target)
    {
        target.SetCustomProperties(new Hashtable() { { "IS_DEAD", true } });
    }
}