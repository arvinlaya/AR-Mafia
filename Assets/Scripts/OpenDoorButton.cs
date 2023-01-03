using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class OpenDoorButton : MonoBehaviour
{
    Vector3 offset = new Vector3(0, 2, 0);
    public HouseController house { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition += offset;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == gameObject.transform)
                {
                    Debug.Log("OPEN DOOR: " + house);
                }
            }
        }

    }
}
