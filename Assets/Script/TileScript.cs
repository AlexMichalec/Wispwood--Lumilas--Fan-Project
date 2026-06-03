using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool inPond = false;
    public int wispType;
    public int pondIndex;
    public bool isEnemy = false;
    public int spawnNumber = 0;
    private Color oldColor = new Color(0, .203f, .487f);
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        if (!inPond || !gameManager.inputEnabled) return;
        if (Time.timeScale == 0) return;
        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        gameManager.ChooseWisp(gameObject);
        GetComponent<Renderer>().material.color = oldColor;
    }

    private void OnMouseEnter()
    {
        if (!inPond || !gameManager.inputEnabled) return;
        oldColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.deepSkyBlue;
    }

    private void OnMouseExit()
    {
        if (!inPond || !gameManager.inputEnabled) return;
        GetComponent<Renderer>().material.color = oldColor;
    }

}
