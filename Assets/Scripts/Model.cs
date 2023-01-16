using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Model : MonoBehaviour, IPunInstantiateMagicCallback
{

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PhotonView parentPV = PhotonView.Find((int)info.photonView.InstantiationData[0]);
        PlayerController parentController = parentPV.GetComponent<PlayerController>();
        gameObject.transform.SetParent(parentController.transform);
        gameObject.transform.position = parentController.transform.position;
    }
}
