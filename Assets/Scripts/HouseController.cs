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
    LeftButton leftButton;
    RightButton rightButton;
    [SerializeField] LeftButton LeftButtonPrefab;
    [SerializeField] RightButton RightButtonPrefab;
    public PhotonAnimatorView animatorView;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        animatorView = GetComponent<PhotonAnimatorView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        leftButton = Instantiate(LeftButtonPrefab, transform.position, Quaternion.identity);
        rightButton = Instantiate(RightButtonPrefab, transform.position, Quaternion.identity);

        leftButton.house = this;
        leftButton.owner = PV.Owner;

        rightButton.house = this;
        rightButton.owner = PV.Owner;
    }

    public void DoorEvent(PhotonView pv)
    {
        if (pv == this.PV)
        {
            openDoor();
        }
        else
        {
            if (animator.GetBool("isOpen") == true)
            {
                closeDoor();
            }
        }
    }

    public void openDoor()
    {
        animator.SetBool("isOpen", true);
    }

    public void closeDoor()
    {
        animator.SetBool("isOpen", false);
    }

    public void showButton()
    {
        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);
    }

    public void hideButton()
    {
        leftButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
    }

}
