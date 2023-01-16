using Photon.Realtime;
interface Role
{
    public string ROLE_TYPE { get; set; }

    public void skill(Player target);
}