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
    [SerializeField] public Material houseMaterialInside;
    [SerializeField] private BoxCollider houseWholeCollider;
    [SerializeField] private BoxCollider houseFrontCollider;
    public MeshRenderer houseRenderer;
    public GameObject houseRoof;
    public MeshRenderer houseRoofRenderer;
    public SkinnedMeshRenderer doorRenderer;
    private bool isOutlined;
    private bool isHidden;
    private PlayerController ownerPlayerController;
    public int outsiderCount;

    void Awake()
    {
        gameObject.tag = "House";
        gameObject.GetComponent<Outline>().enabled = false;
        animator = GetComponent<Animator>();
        doorRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        houseRenderer = transform.GetChild(2).GetComponent<MeshRenderer>();
        houseRoof = transform.GetChild(3).gameObject;
        houseRoofRenderer = houseRoof.GetComponent<MeshRenderer>();

        isOutlined = false;
        isHidden = false;
        outsiderCount = 0;
        PV = GetComponent<PhotonView>();
        ownerPlayerController = PlayerManager.getPlayerController(PV.Owner);
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
        closeDoor();

        if (pv == this.PV)
        {
            openDoor();
        }
    }

    public void openDoor()
    {
        animator.SetBool("isOpen", true);
    }

    public void closeDoor()
    {
        if (!ownerPlayerController)
        {
            ownerPlayerController = PlayerManager.getPlayerController(PV.Owner);
        }
        animator.SetBool("isOpen", false);

        ownerPlayerController.setIdle();
    }

    public void showButtonLeft()
    {
        leftButton.transform.position = transform.position;
        leftButton.transform.Translate(leftButton.offset, transform);
        leftButton.gameObject.SetActive(true);
    }

    public void showButtonRight()
    {
        rightButton.transform.position = transform.position;
        rightButton.transform.Translate(rightButton.offset, transform);
        rightButton.gameObject.SetActive(true);
    }

    public void hideButtonBoth()
    {
        leftButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
    }

    public void startInsideHouse()
    {
        doorRenderer.sharedMaterial = houseMaterialInside;
        houseRenderer.sharedMaterial = houseMaterialInside;

        houseRoof.SetActive(false);

        houseWholeCollider.enabled = false;
        houseFrontCollider.enabled = true;

        gameObject.layer = ReferenceManager.Instance.LayerInsideHouse;
    }

    public void startFadeHouse()
    {
        doorRenderer.sharedMaterial = houseMaterialFade;
        houseRenderer.sharedMaterial = houseMaterialFade;

        houseRoof.SetActive(true);
        houseRoofRenderer.sharedMaterial = houseMaterialFade;

        gameObject.layer = ReferenceManager.Instance.LayerIgnoreRaycast;
        isHidden = true;
    }

    public void startUnfadeHouse()
    {
        doorRenderer.sharedMaterial = houseMaterial;
        houseRenderer.sharedMaterial = houseMaterial;

        houseRoof.SetActive(true);
        houseRoofRenderer.sharedMaterial = houseMaterial;

        houseWholeCollider.enabled = true;
        houseFrontCollider.enabled = false;

        gameObject.layer = ReferenceManager.Instance.LayerHouse;
        isHidden = false;
    }

    private void OnMouseDown()
    {
        PlayerController localController = PlayerManager.getPlayerController(PhotonNetwork.LocalPlayer);

        if (GameManager.Instance.GAME_STATE == GameManager.GAME_PHASE.NIGHT && !PV.IsMine && GameManager.Instance.getAliveList().Contains(PV.Owner))
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("HouseButton"))
            {
                gameObject.SetActive(false);
            }

            if (!PV.IsMine && PV.GetComponent<Transform>().tag == "House")
            {
                showButtonLeft();
                showButtonRight();
            }
        }
    }
    private void OnMouseEnter()
    {
        if (isOutlined == false)
        {
            gameObject.GetComponent<Outline>().enabled = true;
            isOutlined = true;
        }

        TooltipManager.Instance.setHoveredData("House", $"{PV.Owner.NickName}'s House");
    }
    private void OnMouseExit()
    {
        if (isOutlined == true)
        {
            gameObject.GetComponent<Outline>().enabled = false;
            isOutlined = false;
        }

        TooltipManager.Instance.clearHoveredData();
    }
    private void changePhase()
    {
        hideButtonBoth();

        if (GameManager.Instance.getAliveList().Contains(PV.Owner))
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
                if (isHidden == true)
                {
                    startUnfadeHouse();
                }
            }
        }
        else
        {
            if (isHidden == false)
            {
                startFadeHouse();
            }
        }

    }

    public void ignoreRaycast()
    {
        gameObject.layer = ReferenceManager.Instance.LayerIgnoreRaycast;
    }
}
