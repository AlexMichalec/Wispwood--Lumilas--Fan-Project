using UnityEngine;

public class Meowing : MonoBehaviour
{
    public TooltipTrigger tooltip;
    public bool isEvil;
    private string[] meowSamples = {"Meow", "Miau", "Mrrau", "Nya", "Meow, Meow!", "Meoooow", ":3", "*purr, purr*", "*purring*", "*mrrrru*", "Miau, Miau, Miau", ">.<", "Miaaau!", "...", "..", "....", "Meow, Meow, Meow!!!" };
    private string[] evilMeowSamples = { "Mrrrau", "Mrreou", "Mhhrau", "*evil purring*", "Nya, Nya", ">:3", "...", "....", "...", ".....", "..", "*mruczy złowrogo*", "Mhhhrhhrrau", "Miauauauu", "Booo", "Boo!"};
    private float counter = 1;
    void Start()
    {
        tooltip.tooltipContent = GetMeow();
        tooltip.tooltipHeader = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (tooltip.isMouseIn) counter -= Time.deltaTime;
        if (counter <= 0)
        {
            counter = Random.Range(0.5f, 3.0f);
            tooltip.tooltipContent = GetMeow();
            TooltipSystem.Show("", tooltip.tooltipContent);
        }
    }

    string GetMeow()
    {

        return isEvil ? evilMeowSamples[Random.Range(0, evilMeowSamples.Length)] : meowSamples[Random.Range(0, meowSamples.Length)];
    }
}
