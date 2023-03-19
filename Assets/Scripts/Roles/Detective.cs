using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

class Detective : Role
{
    public string ROLE_TYPE { get; set; }


    public Detective()
    {
        ROLE_TYPE = "DETECTIVE";
    }
    public void skill(Player target)
    {
        Debug.Log(target + " ROLE IS: " + target.CustomProperties["ROLE"]);
        LogManager.Instance.addDetectiveAction(PhotonNetwork.LocalPlayer.NickName, target.NickName);
    }
}