using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 cameraPosition;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float sensitivity = 200f;

    [Header("Camera Settings")]
    public float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }


        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("MOVING");
            cameraPosition.z += cameraSpeed / 30;
        }

        if (Input.GetKey(KeyCode.S))
        {
            cameraPosition.z -= cameraSpeed / 30;
        }

        if (Input.GetKey(KeyCode.A))
        {
            cameraPosition.x -= cameraSpeed / 30;
        }

        if (Input.GetKey(KeyCode.D))
        {
            cameraPosition.x += cameraSpeed / 30;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            cameraPosition.y += cameraSpeed / 40;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            cameraPosition.y -= cameraSpeed / 40;
        }

        this.transform.position = cameraPosition;
    }
}
