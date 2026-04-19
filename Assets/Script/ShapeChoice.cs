using UnityEngine;
using UnityEngine.Rendering;

public class ShapeChoice : MonoBehaviour
{
    public GameObject choiceMarker;
    public bool isActive = false;
    public int shapeType;
    public Material standardChoiceMaterial;
    public Material mouseEnterMaterial;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseEnter()
    {
        if (!isActive) return;
        choiceMarker.GetComponent<Renderer>().material = mouseEnterMaterial;
    }

    private void OnMouseDown()
    {
        if (!isActive) return;
        GameObject.Find("GameManager").GetComponent<GameManager>().ChooseShape(shapeType);
    }

    private void OnMouseExit()
    {
        if (!isActive) return;
        choiceMarker.GetComponent<Renderer>().material = standardChoiceMaterial;
    }

    public void Activate()
    {
        isActive = true;
        choiceMarker.SetActive(true);
    }

    public void Deactivate()
    {
        isActive = false;
        choiceMarker.SetActive(false);
    }
}
