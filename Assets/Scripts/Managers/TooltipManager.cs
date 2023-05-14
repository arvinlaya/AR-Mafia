using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameObject tooltip;
    public TMP_Text tooltipHeader;
    public TMP_Text tooltipBody;
    private bool isMouseHovered;
    private string houseOwnerName;
    private string headerMessage;
    private string bodyMessage;

    private float mouseX;
    private float mouseY;
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
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (isMouseHovered)
        {
            tooltip.SetActive(true);
            tooltipHeader.SetText(headerMessage);
            tooltipBody.SetText(bodyMessage);

            Vector2 position = Input.mousePosition;
            position.y += 200f;
            position.x += 40f;

            tooltip.transform.position = position;
            // GameManager.Instance.rotateToCamera(tooltip.transform, ReferenceManager.Instance.camera.transform);
        }
    }

    public void setHoveredData(string headerMessage, string bodyMessage)
    {
        isMouseHovered = true;
        this.headerMessage = headerMessage;
        this.bodyMessage = bodyMessage;
    }

    public void clearHoveredData()
    {
        isMouseHovered = false;
        this.headerMessage = "";
        this.bodyMessage = "";
        tooltip.SetActive(false);
    }
}
