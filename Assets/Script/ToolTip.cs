using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public GameObject toolTipPrefab;
    public RectTransform rectTransform;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    public LayoutElement layoutElement;
    public LayoutElement layoutElementHeader;
    public int charachterLimit = 80;
    public float offsetUp = 15;
    public float offsetDown = -15;

    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string newHeader, string newContent)
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateTooltipPosition();

        headerText.text = newHeader;
        contentText.text = newContent;
        layoutElement.enabled = (newContent.Length > charachterLimit);
        if (newHeader.Length > charachterLimit)
        {
            layoutElementHeader.enabled = true;
            headerText.alignment = TMPro.TextAlignmentOptions.Left;
        }
        else 
        {
            layoutElementHeader.enabled = false;
            headerText.alignment = TMPro.TextAlignmentOptions.Center;
        }
        headerText.gameObject.SetActive(!string.IsNullOrEmpty(newHeader));

    }

    private void Update()
    {
        UpdateTooltipPosition();

    }

    private void UpdateTooltipPosition()
    {
        Vector2 pos = Input.mousePosition;

        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = pos;
        transform.position += new Vector3(0, (pos.y < Screen.height / 2) ? offsetUp : offsetDown, 0);
    }
}
