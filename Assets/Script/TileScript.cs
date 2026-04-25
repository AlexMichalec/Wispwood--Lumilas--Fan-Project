using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool inPond = false;
    public int wispType;
    public int pondIndex;
    public bool isEnemy = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void PutInPond()
    {
        inPond = true;
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
    }

    private void OnMouseDown()
    {
        if (!inPond) return;
        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        GameObject.Find("GameManager").GetComponent<GameManager>().ChooseWisp(gameObject);
    }
}
