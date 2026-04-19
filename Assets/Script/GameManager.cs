using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] tilePrefabs;
    public float radius;
    public int tileAmount = 40;
    public int stacksAmount = 8;
    public float intervalBetweentiles = 0.2f;
    public bool flipped = true;
    public int round = 1;
    public GameObject gridManager;
    public GameObject UINode;
    public GameObject[] spawnPlatforms;
    private List<List<GameObject>> tileStacks = new List<List<GameObject>>();
    public float pondFlipTime = 0.5f;
    public float pondFlipMaxHeight = 1.5f;
    private List<GameObject> pondTiles = new List<GameObject>();
    public List<GameObject> shapeChoicePrefabs;
    private List<GameObject> shapeChoices = new List<GameObject>();
    public float shapeChoicesRadius = 1;
    public float shapeChoiceHeight = 0.1f;
    private GameObject chosenWisp;
    public bool isCatHidden = false;

    void Start()
    {
        StartCoroutine(SpawnTiles());
        StartCoroutine(SpawnShapeChoices());
    }

    IEnumerator SpawnShapeChoices()
    {
        yield return new WaitForEndOfFrame();

        List <int> indexList = new List<int>();
        for (int i = 0; i< 8; ++i)
        {
            int a = Random.Range(0, 8);
            while (indexList.Contains(a)) a = Random.Range(0, 8);
            indexList.Add(a);
        }
        print("IndexList " + indexList);

        for (int i = 0; i < 8; ++i)
        {
            int index = indexList[i];
            Quaternion rot = Quaternion.Euler(0, i*45f + 22.5f, 0);
            Vector3 pos = new Vector3(shapeChoicesRadius, shapeChoiceHeight, 0);
            GameObject newChoice = Instantiate(shapeChoicePrefabs[index], rot * pos, rot);
            shapeChoices.Add(newChoice);
        }
    }

    IEnumerator SpawnTiles()
    {
        for (int i = 0; i < stacksAmount; ++i)
        {
            tileStacks.Add(new List<GameObject>());
        }

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
               // Debug.Log(i+ " "+ stackIndex+ " "+ stackAngle);
                Quaternion tileRotation = Quaternion.Euler(0, stackAngle, 180);
                if (flipped) tileRotation = Quaternion.identity; 

                Vector3 tilePosition = Quaternion.Euler(0, stackAngle, 0) * new Vector3(radius, tileHeight ,0);
                GameObject newTile = Instantiate(tilePrefabs[colorIndex], tilePosition, tileRotation);
                tileStacks[stackIndex%tileStacks.Count].Add(newTile);

            }
        }
        
        //for (int i = 0; i<spawnPlatforms.Length; i++)
        //{
        //    Instantiate(tilePrefabs[Random.Range(0,4)], spawnPlatforms[i].transform.position + new Vector3(0,0.6f,0), Quaternion.Euler(0,180,0));
        //}
        
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NewRound()
    {
        round++;
        gridManager.GetComponent<GridManager>().Deforest();
        gridManager.GetComponent<GridManager>().maxDimension += 1;
        UINode.GetComponent<UI>().UpdateTopText("Runda " + round + "\n" + (round + 3) + "x" + (round + 3));
        UINode.GetComponent<UI>().ResetDetailedScore();
    }

    IEnumerator PondFlip(GameObject flippedTile, Vector3 newPosition, float flipTime, float maxHeight)
    {
        float counter = 0;
        flippedTile.GetComponent<Rigidbody>().mass = 0;
        Vector3 oldPosition = flippedTile.transform.position;
        Quaternion oldRotation = flippedTile.transform.rotation;
        Quaternion newRotation = flippedTile.transform.rotation * Quaternion.Euler(180, 0, 0);
        while (counter < flipTime)
        {
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / flipTime);

            float newPosX = Mathf.Lerp(oldPosition.x, newPosition.x, t);
            float newPosZ = Mathf.Lerp(oldPosition.z, newPosition.z, t);

            float height = Mathf.Sin(Mathf.PI * t);

            float newPosY = Mathf.Lerp(oldPosition.y, newPosition.y, t)
                      + height * (maxHeight - oldPosition.y);

            flippedTile.transform.position = new Vector3(newPosX, newPosY, newPosZ);

            float easedT = Mathf.SmoothStep(0f, 1f, t);
            flippedTile.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, easedT);
            
            yield return null;
        }
        flippedTile.transform.position = newPosition;
        flippedTile.transform.rotation = newRotation;
        flippedTile.GetComponent<Rigidbody>().mass = 2;
        flippedTile.GetComponent<TileScript>().PutInPond();
    }

    IEnumerator PondFlipAll()
    {
        for (int i = 0; i < pondTiles.Count; i++) Destroy(pondTiles[i]);
        pondTiles.Clear();

        for (int i = 0; i < spawnPlatforms.Length; i++)
        {
            int stackIndex = Random.Range(0, tileStacks.Count);
            GameObject randomTile = tileStacks[stackIndex][Random.Range(0, tileStacks[stackIndex].Count)];
            pondTiles.Add(randomTile);
            randomTile.GetComponent<TileScript>().pondIndex = i;
            tileStacks[stackIndex].Remove(randomTile);
            StartCoroutine(PondFlip(randomTile, spawnPlatforms[i].transform.position + new Vector3(0, 0.2f, 0), pondFlipTime,pondFlipMaxHeight));
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ChooseWisp(GameObject WispTile)
    {
        int pondIndex = WispTile.GetComponent<TileScript>().pondIndex;
        int indexBefore = (pondIndex + 1) % shapeChoices.Count;
        int indexAfter = (pondIndex + 2) % shapeChoices.Count; ;
        shapeChoices[indexBefore].GetComponent<ShapeChoice>().Activate();
        shapeChoices[indexAfter].GetComponent<ShapeChoice>().Activate();
        chosenWisp = WispTile;
        GameObject.Find("UI").GetComponent<UI>().ShowCatShapesButton();

    }

    public void ChooseShape(int shapeType)
    {
        for (int i = 0; i < shapeChoices.Count; i++)
        {
            shapeChoices[i].GetComponent<ShapeChoice>().Deactivate();

        }
        Destroy(chosenWisp);
        print("CHOICE " + shapeType);
        GameObject.Find("Main Camera").GetComponent<MoveCamera>().changePosition();
        GameObject.Find("Forest").GetComponent<GridManager>().AddNewShape(shapeType, chosenWisp.GetComponent<TileScript>().wispType);
        GameObject.Find("UI").GetComponent<UI>().HidePondActions();
    }

    public void TreeTurn()
    {
        GameObject.Find("Main Camera").GetComponent<MoveCamera>().changePosition();
        GameObject.Find("Forest").GetComponent<GridManager>().TreeTurn();
        if (isCatHidden) StartCoroutine(flipCatLate());
        isCatHidden = false;
        GameObject.Find("UI").GetComponent<UI>().HidePondActions();

    }

    IEnumerator flipCatLate()
    {
        GameObject.Find("UI").GetComponent<UI>().flipCat();
        yield return new WaitForSeconds(2);
        StartCoroutine(GameObject.Find("Forest").GetComponent<GridManager>().FlipCatTile());
        
    }

    public void CatActionNewWisps()
    {
        isCatHidden = true;
        GameObject.Find("UI").GetComponent<UI>().flipCat();
        StartCoroutine(GameObject.Find("Forest").GetComponent<GridManager>().FlipCatTile());
        StartCoroutine(PondFlipAll());
    }

    public void CatActionAllShapes()
    {
        isCatHidden = true;
        GameObject.Find("UI").GetComponent<UI>().flipCat();
        StartCoroutine(GameObject.Find("Forest").GetComponent<GridManager>().FlipCatTile());
        for (int i = 0; i < shapeChoices.Count; ++i)
        {
            shapeChoices[i].GetComponent<ShapeChoice>().Activate();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(PondFlipAll());
        }
    }
}
