using UnityEngine;

public class Clickable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        print("TEST");
    }

    private void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = new Color(0,0.4472f,1);
    }
}
