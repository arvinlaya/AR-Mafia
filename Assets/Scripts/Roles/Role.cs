using Photon.Realtime;
using UnityEngine;
interface Role
{
    public string ROLE_TYPE { get; set; }

    public void skill(Player player);
}