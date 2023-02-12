using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

class Doctor : Role
{
    public string ROLE_TYPE { get; set; }

    public Doctor()
    {
        ROLE_TYPE = "DOCTOR";
    }
    public void skill(Player target)
    {
        Debug.Log(target.NickName + "IS SAVED");
    }
}