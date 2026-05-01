using UnityEngine;

public class TooltipSystem : MonoBehaviour
{

    private static TooltipSystem current;

    public ToolTip tooltip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        current = this;
    }

    public static void Show(string header, string content)
    {
        if (current.tooltip == null) return;
        current.tooltip.SetText(header, content);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (current.tooltip == null) return;
        current.tooltip.gameObject.SetActive(false);
    }
}
