using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RaycastScript : MonoBehaviour
{
    [SerializeField] GameObject spawnPoint5;
    [SerializeField] GameObject spawnPoint6;
    [SerializeField] GameObject spawnPoint7;
    [SerializeField] GameObject spawnPoint8;
    public GameObject spawnPrefab;
    [SerializeField] ARAnchorManager aRAnchorManager;
    GameObject spawnedObject;
    bool objectSpawned;
    ARRaycastManager arrayManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update
    void Start()
    {
        objectSpawned = false;
        arrayManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (arrayManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitpose = hits[0].pose;
                ARAnchor anchor = aRAnchorManager.AddAnchor(hits[0].pose);
                spawnPoint5.transform.SetParent(anchor.transform);
            }
        }
    }
}
