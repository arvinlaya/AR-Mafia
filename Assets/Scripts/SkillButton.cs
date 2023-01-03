using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class SkillButton : MonoBehaviour
{
    Vector3 offset = new Vector3(1.2f, 2, 0);
    public Player owner { get; set; }



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
                    Debug.Log("USE SKILL: " + owner);
                }
            }
        }

    }
}
