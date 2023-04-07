using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BigUmbrella : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickDC()
    {
        PhotonNetwork.Disconnect();
    }

}
