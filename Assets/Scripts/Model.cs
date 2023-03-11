using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

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
        parentController.animator = GetComponent<Animator>();
        if (parentController.transform.childCount == 0)
        {
            gameObject.transform.SetParent(parentController.transform);
            gameObject.transform.position = parentController.transform.position;
            parentController.animator = gameObject.GetComponent<Animator>();
        }
        else
        {
            Destroy(gameObject);
        }

        if (PV.IsMine)
        {
            Destroy(gameObject);
            GameObject newModel = null;
            switch (PhotonNetwork.LocalPlayer.CustomProperties["ROLE"])
            {
                case "VILLAGER":
                    newModel = Instantiate(ReferenceManager.Instance.Models[0]);
                    break;

                case "MAFIA":
                    newModel = Instantiate(ReferenceManager.Instance.Models[1]);
                    break;

                case "DOCTOR":
                    newModel = Instantiate(ReferenceManager.Instance.Models[2]);
                    break;

                case "DETECTIVE":
                    newModel = Instantiate(ReferenceManager.Instance.Models[3]);
                    break;

                default:
                    Debug.Log("MESH ERROR");
                    break;
            }
            parentController.animator = newModel.GetComponent<Animator>();
            parentController.animationSync = newModel.GetComponent<PhotonAnimatorView>();
            newModel.transform.SetParent(parentController.transform, true);
            newModel.transform.position = parentController.transform.position;
        }


    }
}
