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

    [Header("Tiles - Spawning")]
    public GameObject[] tilePrefabs;
    public float radius;
    public int tileAmount = 40;
    public int stacksAmount = 8;
    public float intervalBetweenTiles = 0.2f;
    

    [Header("Pond - Spawning")]
    public GameObject[] spawnPlatforms;
    public float pondFlipTime = 0.5f;
    public float pondFlipMaxHeight = 1.5f;

    private List<List<GameObject>> tileStacks = new List<List<GameObject>>();
    private List<GameObject> pondTiles = new List<GameObject>();

    [Header("Shapes - Spawning")]
    public List<GameObject> shapeChoicePrefabs;
    private List<GameObject> shapeChoices = new List<GameObject>();
    public float shapeChoicesRadius = 1;
    public float shapeChoiceHeight = 0.1f;
    
    [Header("Others")]
    public int round = 1;
    public bool isCatHidden = false;
    public bool singlePlayer = true;
    private GameObject chosenWisp;
    private bool enemySpawned = false;

    [Header("Navigation")]
    public GridManager gridManager;
    public UI userInterface;
    public MoveCamera cameraMover;
    public EnemyManager enemyManager;


    void Start()
    {
        if (!userInterface.testingMenu)StartGame();
    }

    public void StartGame()
    {
        userInterface.HideDealNewWisps();
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
            newChoice.GetComponent<ShapeChoice>().gameManager = this;
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
                float tileHeight = 0.2f + intervalBetweenTiles * (i - stackIndex * (tileAmount / stacksAmount));
               // Debug.Log(i+ " "+ stackIndex+ " "+ stackAngle);
                Quaternion tileRotation = Quaternion.Euler(0, stackAngle, 180);

                Vector3 tilePosition = Quaternion.Euler(0, stackAngle, 0) * new Vector3(radius, tileHeight ,0);
                GameObject newTile = Instantiate(tilePrefabs[colorIndex], tilePosition, tileRotation);
                tileStacks[stackIndex%tileStacks.Count].Add(newTile);

            }
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(PondFlipAll());

        //for (int i = 0; i<spawnPlatforms.Length; i++)
        //{
        //    Instantiate(tilePrefabs[Random.Range(0,4)], spawnPlatforms[i].transform.position + new Vector3(0,0.6f,0), Quaternion.Euler(0,180,0));
        //}
        
    }

    void SpawnEnemyTile(int index = -1)
    {
        if (index == -1) index = Random.Range(0, pondTiles.Count - 1);
        Vector3 pos = pondTiles[index].transform.position;
        Destroy(pondTiles[index]);
        enemyManager.SpawnEnemy(pos, index);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NewRound()
    {
        round++;
        gridManager.Deforest();
        gridManager.movingCat = false;
        gridManager.maxDimension += 1;
        userInterface.UpdateTopText("Runda " + round + "\n" + (round + 3) + "x" + (round + 3));
        userInterface.ResetDetailedScore();
        StartCoroutine(NewRoundCoroutine());
        
    }

    IEnumerator NewRoundCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        cameraMover.changePosition();
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

   public IEnumerator PondFlipAll()
    {
        for (int i = 0; i < pondTiles.Count; i++)
        {
            if (pondTiles[i] != null && pondTiles[i].GetComponent<TileScript>().isEnemy) continue;
            Destroy(pondTiles[i]); 
        }

        if (pondTiles.Count == 0) pondTiles = new List<GameObject> { null, null, null, null, null, null, null, null };

        for (int i = 0; i < spawnPlatforms.Length; i++)
        {
            if (pondTiles[i] != null) continue;
            int stackIndex = Random.Range(0, tileStacks.Count);
            GameObject randomTile = tileStacks[stackIndex][tileStacks[stackIndex].Count - 1];// Random.Range(0, tileStacks[stackIndex].Count)];
            pondTiles[i] = randomTile;
            randomTile.GetComponent<TileScript>().pondIndex = i;
            tileStacks[stackIndex].Remove(randomTile);
            StartCoroutine(PondFlip(randomTile, spawnPlatforms[i].transform.position + new Vector3(0, 0.2f, 0), pondFlipTime,pondFlipMaxHeight));
            yield return new WaitForSeconds(0.1f);
        }

        if (singlePlayer && !enemySpawned)
        {
            yield return new WaitForSeconds(pondFlipTime);
            enemySpawned = true;
            SpawnEnemyTile();
        }
    }

    public void DealNewWisps()
    {
        //place to put "if (not enough wisps)" in future
        StartCoroutine(PondFlipAll());
    }

    public bool CanDealNewWispsForFree()
    {
        //Player can deal new wisps using cat action before their turn or for free when:
        //a) all places for wisps in pond are empty or
        bool isEmpty = true;
        //b) all wisps in the pond are the same type
        bool isTheSameType = true;
        int firstType = -1;
        for (int i = 0; i < pondTiles.Count; ++i)
        {
            if (pondTiles[i] == null || pondTiles[i].GetComponent<TileScript>().isEnemy) continue;
            isEmpty = false;
            int wispType = pondTiles[i].GetComponent<TileScript>().wispType;
            if (firstType == -1) firstType = wispType;
            else if (firstType != wispType) isTheSameType = false;
        }

        return isEmpty || isTheSameType;
    }

    public void ChooseWisp(GameObject WispTile)
    {
        int pondIndex = WispTile.GetComponent<TileScript>().pondIndex;
        int indexBefore = (pondIndex + 1) % shapeChoices.Count;
        int indexAfter = (pondIndex + 2) % shapeChoices.Count; ;
        shapeChoices[indexBefore].GetComponent<ShapeChoice>().Activate();
        shapeChoices[indexAfter].GetComponent<ShapeChoice>().Activate();
        chosenWisp = WispTile;
        userInterface.ShowCatShapesButton();

    }

    public void ChooseShape(int shapeType)
    {
        for (int i = 0; i < shapeChoices.Count; i++)
        {
            shapeChoices[i].GetComponent<ShapeChoice>().Deactivate();

        }
        chosenWisp.SetActive(false);
        print("CHOICE " + shapeType);
        cameraMover.changePosition();
        gridManager.AddNewShape(shapeType, chosenWisp.GetComponent<TileScript>().wispType);
        userInterface.HidePondActions();
        userInterface.ShowArrowsForShapes();
    }

    public void UndoChoice()
    {
        chosenWisp.SetActive(true);
        chosenWisp.GetComponent<TileScript>().PutInPond();
        chosenWisp = null;

    }

    public void FinalizeChoice()
    {
        Destroy(chosenWisp);
        chosenWisp = null;
    }

    public void FinishRound()
    {
        gridManager.SubmitGrid();
        gridManager.movingCat = true;
        userInterface.ShowArrows();
        userInterface.HideTreeTurnActions();
        userInterface.ShowYouCanMoveCat();
    }

    public void TreeTurn()
    {
        cameraMover.changePosition();
        gridManager.TreeTurn();
        if (isCatHidden) StartCoroutine(flipCatLate());
        isCatHidden = false;
        userInterface.HidePondActions();

    }

    IEnumerator flipCatLate()
    {
        userInterface.flipCat();
        yield return new WaitForSeconds(2);
        StartCoroutine(gridManager.FlipCatTile());
        
    }

    public void CatActionNewWisps()
    {
        isCatHidden = true;
        userInterface.flipCat();
        StartCoroutine(gridManager.FlipCatTile());
        StartCoroutine(PondFlipAll());
    }

    public void CatActionAllShapes()
    {
        isCatHidden = true;
        userInterface.flipCat();
        StartCoroutine(gridManager.FlipCatTile());
        for (int i = 0; i < shapeChoices.Count; ++i)
        {
            shapeChoices[i].GetComponent<ShapeChoice>().Activate();
        }
    }

    public List<GameObject> GetPondTiles()
    {
        return pondTiles;
    }

    public void NextPlayer(bool lastTurn = false)
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(PondFlipAll());
        }
    }
}
