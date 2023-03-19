using Photon.Realtime;
using Photon.Pun;
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
        target.SetCustomProperties(new Hashtable() { { "IS_SAVED", true } });
        LogManager.Instance.addDoctorAction(PhotonNetwork.LocalPlayer.NickName, target.NickName);
    }
}