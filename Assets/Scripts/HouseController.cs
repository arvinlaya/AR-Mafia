using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;


public class HouseController : MonoBehaviour
{

    public PhotonView PV { get; set; }

    Animator animator;
    void Awake()
    {
        PV = GetComponent<PhotonView>();

    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        LeftButton.OnDoorEvent += DoorEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DoorEvent(PhotonView pv)
    {
        if (pv == this.PV)
        {
            animator.SetBool("isOpen", true);
        }
        else
        {
            if (animator.GetBool("isOpen") == true)
            {
                animator.SetBool("isOpen", false);
            }
        }
    }
}
