using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float sensitivity = 200f;
    private Vector3 cameraPosition;
    private Vector2 turn;
    [Header("Camera Settings")]
    public float cameraSpeed;
    public static CameraController Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Debug.Log("THIS IS PC");
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            turn.x += Input.GetAxis("Mouse X");
            turn.y += Input.GetAxis("Mouse Y");
            transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
        }

        float playerVerticalInput = Input.GetAxis("Vertical") * cameraSpeed;
        float playerHorizontalInput = Input.GetAxis("Horizontal") * cameraSpeed;

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 forwardRelativeHorizontalInput = playerHorizontalInput * right;

        this.transform.position += forwardRelativeVerticalInput + forwardRelativeHorizontalInput;

        cameraPosition.y = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            cameraPosition.y += cameraSpeed;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            cameraPosition.y -= cameraSpeed;
        }

        this.transform.position += cameraPosition;
    }


}
