using Photon.Realtime;
using UnityEngine;

class Police : Role
{
    public string ROLE_TYPE { get; set; }


    public Police()
    {
        ROLE_TYPE = "POLICE";
    }
    public void skill(Player target)
    {
        Debug.Log(target + " ROLE IS: " + target.CustomProperties["ROLE"]);
    }
}