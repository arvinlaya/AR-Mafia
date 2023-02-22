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

    [SerializeField] public Transform ownerLocation;
    [SerializeField] public Transform ownerFront;
    [SerializeField] public Transform houseFront;
    [SerializeField] public Transform[] outsiderLocation;
    [SerializeField] public Material houseMaterial;
    [SerializeField] public Material houseMaterialFade;
    private bool isOutlined;
    private bool isHidden;
    private float fadeSpeed = .05f;
    private Renderer houseRenderer;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameObject.GetComponent<Outline>().enabled = false;
        animator = GetComponent<Animator>();
        houseRenderer = GetComponent<Renderer>();
        isOutlined = false;
        isHidden = false;
    }
    // Start is called before the first frame update
    void Start()
    {

        leftButton = Instantiate(LeftButtonPrefab, transform.position, Quaternion.identity);
        rightButton = Instantiate(RightButtonPrefab, transform.position, Quaternion.identity);

        leftButton.house = this;
        leftButton.owner = PV.Owner;

        rightButton.house = this;
        rightButton.owner = PV.Owner;

        GameManager.Instance.OnPhaseChange += changePhase;
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

    public void showButtonLeft()
    {
        leftButton.gameObject.SetActive(true);
    }

    public void showButtonRight()
    {
        rightButton.gameObject.SetActive(true);
    }

    public void hideButtonBoth()
    {
        leftButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
    }

    public void startFadeHouse()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = houseMaterialFade;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = houseMaterialFade;
        gameObject.layer = ReferenceManager.Instance.LayerIgnoreRaycast;
        isHidden = true;
    }

    public void startUnfadeHouse()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = houseMaterial;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = houseMaterial;
        gameObject.layer = ReferenceManager.Instance.LayerIgnoreRaycast;
        isHidden = false;
    }

    private void OnMouseOver()
    {
        if (isOutlined == false)
        {
            gameObject.GetComponent<Outline>().enabled = true;
            isOutlined = true;
        }
    }
    private void OnMouseExit()
    {
        if (isOutlined == true)
        {
            gameObject.GetComponent<Outline>().enabled = false;
            isOutlined = false;
        }
    }
    private void changePhase()
    {
        if (GameManager.Instance.GAME_STATE != GameManager.GAME_PHASE.NIGHT)
        {
            if (isHidden == false)
            {
                startFadeHouse();
            }
        }
        else
        {
            if (isHidden == false)
            {
                startUnfadeHouse();
            }
        }
    }
}
