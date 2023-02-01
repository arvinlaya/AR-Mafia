using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Model : MonoBehaviour, IPunInstantiateMagicCallback
{
    PhotonView PV;

    void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        PhotonView parentPV = PhotonView.Find((int)info.photonView.InstantiationData[0]);
        PlayerController parentController = parentPV.GetComponent<PlayerController>();
        if (parentController.transform.childCount == 0)
        {
            gameObject.transform.SetParent(parentController.transform);
            gameObject.transform.position = parentController.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
