using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipHeader;
    public string tooltipContent;
    [HideInInspector]
    public bool isMouseIn = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseIn = true;
        StartCoroutine(delayShowTiptool());
    }

    IEnumerator delayShowTiptool()
    {
        yield return new WaitForSeconds(0.5f);
        if (isMouseIn) TooltipSystem.Show(tooltipHeader, tooltipContent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseIn = false;
        TooltipSystem.Hide();
    }

    private void OnDisable()
    {
        if (!isMouseIn) return;
        isMouseIn = false;
        TooltipSystem.Hide();
    }

    private void OnMouseEnter()
    {
        isMouseIn = true;
        StartCoroutine(delayShowTiptool());
    }

    private void OnMouseExit()
    {
        isMouseIn = false;
        TooltipSystem.Hide();
    }
}
