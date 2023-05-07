using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RaycastScript : MonoBehaviour
{
    [SerializeField] GameObject spawnManager;
    public GameObject spawnPrefab;
    [SerializeField] ARAnchorManager aRAnchorManager;
    GameObject spawnedObject;
    bool objectSpawned;
    bool isInitialized;
    ARRaycastManager arrayManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update
    void Start()
    {
        objectSpawned = false;
        arrayManager = GetComponent<ARRaycastManager>();
        isInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            if (Input.touchCount > 0)
            {
                if (arrayManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitpose = hits[0].pose;
                    spawnManager.transform.position = hitpose.position;
                    SpawnManager.Instance.SpawnPlayersAndHouses();
                }
            }
        }

    }
}
