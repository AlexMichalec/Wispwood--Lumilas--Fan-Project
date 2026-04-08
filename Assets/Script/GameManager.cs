using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] tilePrefabs;
    public float radius;
    public int tileAmount = 40;
    public int stacksAmount = 8;
    public float intervalBetweentiles = 0.2f;
    public bool flipped = true;
    void Start()
    {
        StartCoroutine(SpawnTiles());
    }

    IEnumerator SpawnTiles()
    {
        List<int> colorList = new List<int>();
        for (int i = 0; i < tileAmount; i++)
        {
            colorList.Add(i % tilePrefabs.Length);
        }
        for (int i = 0; i < tileAmount; i++)
        {
            int a = Random.Range(0, colorList.Count);
            int b = Random.Range(0, colorList.Count);
            int temp = colorList[a];
            colorList[a] = colorList[b];
            colorList[b] = temp;
        }

        if (stacksAmount == 1)
        {
            for (int i = 0; i < tileAmount; i++)
            {
                yield return new WaitForSeconds(0.01f);
                int colorIndex = colorList[i];
                if (colorIndex > 3) colorIndex = 3;
                Vector3 tilePosition = new Vector3(Random.Range(-radius, radius), Random.Range(2, 5), Random.Range(-radius, radius));
                Instantiate(tilePrefabs[colorIndex], tilePosition, Random.rotation);

            }
        }
        else
        {
            for (int i = 0; i < tileAmount; i++)
            {
                if (i%(tileAmount/stacksAmount) == 0) yield return new WaitForSeconds(0.05f);
                int colorIndex = colorList[i];
                int stackIndex = i / (tileAmount / stacksAmount);
                float stackAngle = stackIndex * (360.0f / stacksAmount);
                float tileHeight = 0.2f + intervalBetweentiles * (i - stackIndex * (tileAmount / stacksAmount));
                Debug.Log(i+ " "+ stackIndex+ " "+ stackAngle);
                Quaternion tileRotation = new Quaternion(0, 0, 180, 0);
                if (flipped) tileRotation = Quaternion.identity; 

                Vector3 tilePosition = Quaternion.Euler(0, stackAngle, 0) * new Vector3(radius, tileHeight ,0);
                Instantiate(tilePrefabs[colorIndex], tilePosition, tileRotation);

            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        
    }
}
