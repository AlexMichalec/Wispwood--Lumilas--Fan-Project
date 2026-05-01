using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public GameObject toolTipPrefab;
    public RectTransform rectTransform;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    public LayoutElement layoutElement;
    public int charachterLimit = 80;
    
    void Awake()
    {
        
    }

    public void SetText(string newHeader, string newContent)
    {
        rectTransform = GetComponent<RectTransform>();

        Vector2 pos = Input.mousePosition;

        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = pos;

        headerText.text = newHeader;
        contentText.text = newContent;
        layoutElement.enabled = (newContent.Length > charachterLimit);
        headerText.gameObject.SetActive(!string.IsNullOrEmpty(newHeader));

    }

    private void Update()
    {
        Vector2 pos = Input.mousePosition;

        float pivotX = pos.x / Screen.width;
        float pivotY = pos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = pos;

    }

}
