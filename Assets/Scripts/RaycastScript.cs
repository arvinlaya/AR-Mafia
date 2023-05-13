using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Photon.Pun;
using TMPro;

public class RaycastScript : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameObject placementIndicator;
    public static RaycastScript Instance;
    [SerializeField] private ARSessionOrigin arOrigin;
    private Pose placementPose;
    [SerializeField] GameObject spawnManager;
    [SerializeField] ARAnchorManager aRAnchorManager;
    GameObject spawnedObject;
    bool objectSpawned;
    bool isInitialized;
    bool placementPoseIsValid = false;
    bool isGamePlaced = false;
    ARRaycastManager arrayManager;
    Pose spawnPosition;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] public GameObject waiting;
    [SerializeField] GameObject placementPrompt;
    // Start is called before the first frame update
    Pose lastHitPose;
    int readyCount = 0;
    PhotonView PV;
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        objectSpawned = false;
        arrayManager = GetComponent<ARRaycastManager>();
        isInitialized = false;
        PV = GetComponent<PhotonView>();

        StartCoroutine(nameof(startGame));
    }

    // Update is called once per frame
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            Debug.Log("DEBUG PLACING INDICATOR");
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, rotation: Quaternion.Euler(90f, 0f, 0f));
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PV.RPC(nameof(RPC_setReady), RpcTarget.All);
            isInitialized = true;
        }

        if (isInitialized == false && placementPrompt.activeSelf == false)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (Input.touchCount > 0)
            {
                if (placementPrompt.activeSelf == false)
                {
                    if (arrayManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = hits[0].pose;
                        showPlacementPrompt(hitPose);
                    }
                }
            }
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = ReferenceManager.Instance.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arrayManager.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            Debug.Log("DEBUG PLACEMENT IS VALID");
        }
    }

    public void showPlacementPrompt(Pose pose)
    {
        lastHitPose = pose;
        placementPrompt.SetActive(true);
    }

    public void closePlacementPrompt()
    {
        placementPrompt.SetActive(false);
    }

    public void setGamePlace()
    {
        placementPrompt.SetActive(false);
        placementIndicator.SetActive(false);
        isInitialized = true;

        GameObject anchorObject = new GameObject("Anchor");
        anchorObject.transform.position = lastHitPose.position;
        anchorObject.transform.rotation = lastHitPose.rotation;

        spawnManager.transform.position = lastHitPose.position;
        spawnManager.transform.SetParent(anchorObject.transform);

        waiting.SetActive(true);
        PV.RPC(nameof(RPC_setReady), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_setReady()
    {
        readyCount += 1;
    }

    private IEnumerator startGame()
    {
        yield return new WaitUntil(() => readyCount >= GameManager.Instance.aliveCount);

        placementIndicator.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.Instance.SpawnPlayersAndHouses();
        }
    }
}
