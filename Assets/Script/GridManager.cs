using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine.Rendering;

public class GridManager : MonoBehaviour
{
    [Header("General")]
    public int maxDimension = 4;
    public float distanceBetweenTiles = 0.4f;
    public float spawnHeight = 0.1f;
    public bool movingCat = false;

    [Header("Cat Flipping")]
    public float flipHeight;
    public float flipTime;

    [Header("Prefabs")]
    public GameObject[] tilePrefabs;
    public GameObject catTilePrefab;
    public GameObject choiceTilePrefab;
    public GameObject shapePrefab;
    
    [Header("Navigation")]
    public UI userInterface;
    public MoveCamera cameraMover;
    public Score myScore;
    public GameManager gameManager;

    //Grids
    private List<List<int>> gridList = new List<List<int>>();
    private List<List<GameObject>> gridList2 = new List<List<GameObject>>();

    //Lists
    private List<Vector2> possiblePlacesToAddTile = new List<Vector2>();

    //Objects
    private GameObject catTile;
    private GameObject choiceTile;
    private GameObject currentShape;
    private Vector2Int catLocation = new Vector2Int(-1,-1);
    private bool isCatJumping = false;

    //Variables
    private int treeTurnCounter = 0;
    private int choiceIndex = 0;
    

    //Does it have to be global?
    private Vector2 nearChoiceLocation;
    private float moveCenter = 0.02f;

    //Old versions
    private float flipForce;
    private float rotateForce;


    // 0 - puste, 1 - drzewo, 2 - dynia, 3 - serce, 4 - wiedżma, 5 - duszek, 6 - kot
    void Start()
    {
        InitializeGrid();
        moveCenter = distanceBetweenTiles / 20.0f;
        Debug.Log("MC " + moveCenter);
        StartCoroutine(lateUpdateChoiceTile());

    }

    public void resetCat()
    {
        Vector3 catPos = catTile.transform.position;
        Quaternion catRot = catTile.transform.rotation;
        Destroy(catTile);
        catTile = Instantiate(catTilePrefab, catPos, catRot);
        gridList2[0][0] = catTile;
    }

    IEnumerator lateUpdateChoiceTile()
    {
        yield return new WaitForSeconds(0.5f);
        updateChoiceTile();


    }

    public void SubmitGrid()
    {
        myScore.gridList = gridList;
        myScore.SumUpScore();

    }

    void InitializeGrid()
    {
        gridList.Add(new List<int> { 6 });
        Vector3 catPosition = new Vector3(transform.position.x, spawnHeight, transform.position.z);
        catTile = Instantiate(catTilePrefab, catPosition, new Quaternion(0, 180, 0, 0));
        gridList2.Add(new List<GameObject> { catTile });
    }

    void addTile()
    {
        addRandomTile();
    }



    void setNextChoice()
    {
        if (IsFull()) return;
        choiceIndex = (choiceIndex + 1) % possiblePlacesToAddTile.Count;
        updateChoiceTile();
        return;
    }

    void SetPreviousChoice()
    {
        if (IsFull()) return;
        choiceIndex = choiceIndex - 1;
        if (choiceIndex < 0) choiceIndex = possiblePlacesToAddTile.Count - 1;
        updateChoiceTile();
        return;
    }

    void setDownChoice()
    {
        if (IsFull()) return;
        float old_y = possiblePlacesToAddTile[choiceIndex].y;
        int counter = 0;
        while (old_y == possiblePlacesToAddTile[choiceIndex].y && counter < possiblePlacesToAddTile.Count)
        {
            choiceIndex = (choiceIndex + 1) % possiblePlacesToAddTile.Count;
            counter++;
        }
        updateChoiceTile();
        return;
    }

    void setUpChoice()
    {
        if (IsFull()) return;
        float old_y = possiblePlacesToAddTile[choiceIndex].y;
        int counter = 0;
        while (old_y == possiblePlacesToAddTile[choiceIndex].y && counter < possiblePlacesToAddTile.Count)
        {
            choiceIndex = choiceIndex - 1;
            if (choiceIndex < 0) choiceIndex = possiblePlacesToAddTile.Count - 1;
            counter++;
        }

        updateChoiceTile();
        return;
    }


    void updateChoiceTile()
    {
        if (choiceTile == null)
        {
            choiceTile = Instantiate(choiceTilePrefab);
            PreparePossiblePlaces();
            if (choiceIndex >= possiblePlacesToAddTile.Count)
                choiceIndex = 0;
        }
        int pt_x = (int)possiblePlacesToAddTile[choiceIndex].x;
        int pt_y = (int)possiblePlacesToAddTile[choiceIndex].y;
        //DOWN
        if (pt_x < 0)
        {
            GameObject otherTile = gridList2[pt_x + 1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x - distanceBetweenTiles,
                0.01f, otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x + 1, pt_y);
            return;
        }
        //UP
        if (pt_x == gridList.Count)
        {
            GameObject otherTile = gridList2[pt_x - 1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x + distanceBetweenTiles,
                0.01f, otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x - 1, pt_y);
            return;
        }
        //LEFT
        if (pt_y == gridList[0].Count)
        {
            GameObject otherTile = gridList2[pt_x][pt_y - 1];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x,
                0.01f, otherTile.transform.position.z + distanceBetweenTiles);
            nearChoiceLocation = new Vector2(pt_x, pt_y - 1);
            return;
        }
        //RIGHT
        if (pt_y < 0)
        {
            GameObject otherTile = gridList2[pt_x][pt_y + 1];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x,
                0.01f, otherTile.transform.position.z - distanceBetweenTiles);
            nearChoiceLocation = new Vector2(pt_x, pt_y + 1);
            return;
        }
        //UP (względtem tego u góry)
        if (pt_x > 0 && gridList[pt_x - 1][pt_y] > 0)
        {
            GameObject otherTile = gridList2[pt_x - 1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x + distanceBetweenTiles,
                0.01f, otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x - 1, pt_y);
            return;
        }
        //DOWN
        if (pt_x < gridList.Count - 1 && gridList[pt_x + 1][pt_y] > 0)
        {
            GameObject otherTile = gridList2[pt_x + 1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x - distanceBetweenTiles,
                0.01f, otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x + 1, pt_y);
            return;
        }
        //LEFT
        if (pt_y > 0 && gridList[pt_x][pt_y - 1] > 0)
        {
            GameObject otherTile = gridList2[pt_x][pt_y - 1];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x,
                0.01f, otherTile.transform.position.z + distanceBetweenTiles);
            nearChoiceLocation = new Vector2(pt_x, pt_y - 1);
            return;
        }
        //RIGHT
        if (pt_y < gridList[0].Count && gridList[pt_x][pt_y + 1] > 0)
        {
            GameObject otherTile = gridList2[pt_x][pt_y + 1];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x,
                0.01f, otherTile.transform.position.z - distanceBetweenTiles);
            nearChoiceLocation = new Vector2(pt_x, pt_y + 1);
            return;
        }

    }



    void PreparePossiblePlaces()
    {
        possiblePlacesToAddTile.Clear();
        for (int i = 0; i < gridList.Count; i++)
        {
            for (int j = 0; j < gridList[i].Count; j++)
            {
                if (gridList[i][j] == 0) continue;
                if (CanAddLeft(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i, j - 1)))
                    possiblePlacesToAddTile.Add(new Vector2(i, j - 1));
                if (CanAddRight(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i, j + 1)))
                    possiblePlacesToAddTile.Add(new Vector2(i, j + 1));
                if (CanAddUp(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i - 1, j)))
                    possiblePlacesToAddTile.Add(new Vector2(i - 1, j));
                if (CanAddDown(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i + 1, j)))
                    possiblePlacesToAddTile.Add(new Vector2(i + 1, j));

            }
        }
        //possiblePlacesToAddTile.Sort();
        SortPossiblePlaces();
        return;
    }

    void SortPossiblePlaces()
    {
        for (int i = 0; i < possiblePlacesToAddTile.Count; i++)
        {
            for (int j = i + 1; j < possiblePlacesToAddTile.Count; j++)
            {
                if (possiblePlacesToAddTile[i].y == possiblePlacesToAddTile[j].y)
                {
                    if (possiblePlacesToAddTile[i].x > possiblePlacesToAddTile[j].x)
                    {
                        Vector2 temp = possiblePlacesToAddTile[i];
                        possiblePlacesToAddTile[i] = possiblePlacesToAddTile[j];
                        possiblePlacesToAddTile[j] = temp;
                    }
                }
                else if (possiblePlacesToAddTile[i].y < possiblePlacesToAddTile[j].y)
                {
                    Vector2 temp = possiblePlacesToAddTile[i];
                    possiblePlacesToAddTile[i] = possiblePlacesToAddTile[j];
                    possiblePlacesToAddTile[j] = temp;
                }
            }

        }
    }

    public bool IsFull()
    {
        if (gridList.Count < maxDimension || gridList[0].Count < maxDimension) return false;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                if (gridList[i][j] == 0) return false;
            }
        }
        Debug.Log("Is Full :o");
        return true;

    }

    public void AddTree(int tileIndex = -1)
    {
        if (IsFull()) return;
        if (choiceTile == null) return;
        if (tileIndex < 0) tileIndex = Random.Range(0, 4);
        int gridTileIndex = 1;
        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);
        newTile.transform.rotation = Quaternion.Euler(0, 0, 180);
        Vector2 chosenPlace = possiblePlacesToAddTile[choiceIndex];
        AddNewTile(chosenPlace, newTile, gridTileIndex, nearChoiceLocation);

    }

    void addRandomTile()
    {
        if (IsFull()) return;
        int tileIndex = Random.Range(0, 4);
        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);
        bool isTree = Random.value > 0.5;
        Vector2 usedTile = new Vector2();
        int gridTileIndex = 1;
        if (isTree) newTile.transform.rotation = Quaternion.Euler(0, 0, 180);
        else {
            newTile.transform.rotation = Quaternion.Euler(0, 180, 0);
            gridTileIndex = 2 + tileIndex; }

        //1. znajdź dowolny niezerowy kafelek
        List<Vector2> potentialPlaces = new List<Vector2>();
        while (potentialPlaces.Count == 0)
        {
            int x = Random.Range(0, gridList.Count);
            int y = Random.Range(0, gridList[x].Count);
            while (gridList[x][y] == 0)
            {
                x = Random.Range(0, gridList.Count);
                y = Random.Range(0, gridList[x].Count);

            }
            //2. sprawdź czy ma wolne miejsce na kafelek
            if (CanAddLeft(x, y)) potentialPlaces.Add(new Vector2(x, y - 1));
            if (CanAddRight(x, y)) potentialPlaces.Add(new Vector2(x, y + 1));
            if (CanAddUp(x, y)) potentialPlaces.Add(new Vector2(x - 1, y));
            if (CanAddDown(x, y)) potentialPlaces.Add(new Vector2(x + 1, y));
            usedTile = new Vector2(x, y);
            // jeśli nie wróć do 1.
        }
        // jeśli tak wybierz miejsce gdzie dodać
        Vector2 chosenPlace = potentialPlaces[Random.Range(0, potentialPlaces.Count)];
        AddNewTile(chosenPlace, newTile, gridTileIndex, usedTile);
        //7. wycentruj fizyczną wersję
        printInnerGrid();
        //8* zobacz czy zapełniony?



    }

    public void printInnerGrid()
    {
        string gridString = "\n";
        for (int i = 0; i < gridList.Count; i++)
        {
            string temp = "";
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                temp = temp + gridList[i][j] + ",  ";

            }

            gridString += temp + "\n";
        }
        print(gridString);
        print(gridList.Count + "x" + gridList[0].Count);
        print(gridList2.Count + "x" + gridList2[0].Count);
        print(" ");
    }
    void AddNewTile(Vector2 chosenPlace, GameObject newTile, int gridTileIndex, Vector2 usedTile)
    {
        Debug.Log("BEFORE");
        Debug.Log("ChosenPlace: " + chosenPlace.ToString());
        Debug.Log(gridList.Count + "x" + gridList[0].Count);
        // Rozszerzanie matrycy     <-można o więcej niż 1?
        //w górę
        if (chosenPlace.x < 0)
        {
            List<int> tempList = new List<int>();
            List<GameObject> tempList2 = new List<GameObject>();
            for (int i = 0; i < gridList[0].Count; i++)
            {
                tempList.Add(0);
                tempList2.Add(null);
            }
            gridList.Insert(0, tempList);
            gridList2.Insert(0, tempList2);
            chosenPlace = new Vector2(chosenPlace.x + 1, chosenPlace.y);
            usedTile = new Vector2(usedTile.x + 1, usedTile.y);
            StartCoroutine(CenterGrid(moveCenter, 0));
        }
        //w dół
        if (chosenPlace.x >= gridList.Count)
        {
            List<int> tempList = new List<int>();
            List<GameObject> tempList2 = new List<GameObject>();
            for (int i = 0; i < gridList[0].Count; i++)
            {
                tempList.Add(0);
                tempList2.Add(null);
            }
            gridList.Add(tempList);
            gridList2.Add(tempList2);
            chosenPlace = new Vector2(chosenPlace.x, chosenPlace.y);
            usedTile = new Vector2(usedTile.x, usedTile.y);
            StartCoroutine(CenterGrid(-moveCenter, 0));
        }
        //w lewo
        if (chosenPlace.y < 0)
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].Insert(0, 0);
                gridList2[i].Insert(0, null);
            }
            chosenPlace = new Vector2(chosenPlace.x, chosenPlace.y + 1);
            usedTile = new Vector2(usedTile.x, usedTile.y + 1);
            StartCoroutine(CenterGrid(0, moveCenter));
        }
        //w prawo
        if (chosenPlace.y >= gridList[0].Count)
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].Add(0);
                gridList2[i].Add(null);
            }
            chosenPlace = new Vector2(chosenPlace.x, chosenPlace.y);
            usedTile = new Vector2(usedTile.x, usedTile.y);
            StartCoroutine(CenterGrid(0, -moveCenter));
        }
        //Dodaj kafelek
        Debug.Log("AFTER");
        Debug.Log(chosenPlace.ToString());
        Debug.Log(gridList.Count + "x" + gridList[0].Count);
        gridList[(int)chosenPlace.x][(int)chosenPlace.y] = gridTileIndex;
        gridList2[(int)chosenPlace.x][(int)chosenPlace.y] = newTile;

        //Umieść Fizycznie
        print("used object position: " + usedTile.ToString());
        GameObject usedTileObject = gridList2[(int)usedTile.x][(int)usedTile.y];
        float posX = usedTileObject.transform.position.x + distanceBetweenTiles * (chosenPlace.x - usedTile.x);
        float posY = spawnHeight;
        float posZ = usedTileObject.transform.position.z + distanceBetweenTiles * (chosenPlace.y - usedTile.y);
        newTile.transform.position = new Vector3(posX, posY, posZ);

    }

    bool CanAddLeft(int x, int y)
    {
        return (y >= 1 && gridList[x][y - 1] == 0) || (y == 0) && gridList[0].Count < maxDimension;
    }

    bool CanAddUp(int x, int y)
    {

        return (x >= 1 && gridList[x - 1][y] == 0) || (x == 0) && gridList.Count < maxDimension;
    }

    bool CanAddRight(int x, int y)
    {
        return (y < gridList[0].Count - 1 && gridList[x][y + 1] == 0) || (y == gridList[0].Count - 1) && gridList[0].Count < maxDimension;
    }

    bool CanAddDown(int x, int y)
    {
        return (x < gridList.Count - 1 && gridList[x + 1][y] == 0) || (x == gridList.Count - 1) && gridList.Count < maxDimension;
    }

    IEnumerator CenterGrid(float x, float y)
    {
        yield return new WaitForSeconds(0.2f);
        for (int t = 0; t < 10; t++)
        {
            for (int i = 0; i < gridList2.Count; i++)
            {
                for (int j = 0; j < gridList2[i].Count; j++)
                {
                    if (gridList2[i][j] == null) continue;
                    gridList2[i][j].transform.position += new Vector3(x, 0, y);

                }
            }
            yield return new WaitForSeconds(0.05f);
        }

    }

    public Vector2 GetSize()
    {
        return new Vector2(gridList.Count, gridList[0].Count);
    }

    public void ExtendGrid(int x, int y)
    {
        StartCoroutine(CenterGrid(-x * moveCenter, -y * moveCenter));
        // Rozszerzanie matrycy     <-można o więcej niż 1?
        //w górę
        while (x < 0)
        {
            List<int> tempList = new List<int>();
            List<GameObject> tempList2 = new List<GameObject>();
            for (int i = 0; i < gridList[0].Count; i++)
            {
                tempList.Add(0);
                tempList2.Add(null);
            }
            gridList.Insert(0, tempList);
            gridList2.Insert(0, tempList2);
            //StartCoroutine(CenterGrid(moveCenter, 0));
            x++;
        }
        //w dół
        while (x > 0)
        {
            List<int> tempList = new List<int>();
            List<GameObject> tempList2 = new List<GameObject>();
            for (int i = 0; i < gridList[0].Count; i++)
            {
                tempList.Add(0);
                tempList2.Add(null);
            }
            gridList.Add(tempList);
            gridList2.Add(tempList2);
            --x;
            //StartCoroutine(CenterGrid(-moveCenter, 0));
        }
        //w lewo
        while (y < 0)
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].Insert(0, 0);
                gridList2[i].Insert(0, null);
            }
            y++;
            //StartCoroutine(CenterGrid(0, moveCenter));
        }
        //w prawo
        while (y > 0)
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].Add(0);
                gridList2[i].Add(null);
            }
            //StartCoroutine(CenterGrid(0, -moveCenter));
            y--;
        }
    }
    public void AddTileFromShape(int x, int y, GameObject existingTile, int tileType)
    {
        gridList[x][y] = tileType;
        gridList2[x][y] = existingTile;
    }

    public bool isEmpty(Vector2 location)
    {
        return gridList[(int)location.x][(int)location.y] == 0;
    }

    Vector2 FindCatTile()
    {
        for (int i = 0; i < gridList.Count; i++)
        {
            for (int j = 0; j < gridList[i].Count; j++)
            {
                if (gridList[i][j] == 6) return new Vector2(i, j);
            }
        }
        return new Vector2();
    }

    public void Deforest()
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; j++)
            {
                if (gridList[i][j] == 1)
                {
                    gridList[i][j] = 0;
                    StartCoroutine(DisappearTree(gridList2[i][j]));
                }
            }
        }
    }

    public void ButtonInputManager(int i)
    {
        //If we're adding a shape currently
        if (currentShape != null) 
        {
            currentShape.GetComponent<Shape>().ButtonManager(i);
            return;
        }

        if (movingCat)
        {
            CatInputManager(i);
            return;
        }

        //If already added three trees during current Tree Turn
        if (treeTurnCounter >= 3) return;

        //Do we need that?
        PreparePossiblePlaces();

        //0 LEFT
        if (i == 0)
        {
            PreparePossiblePlaces();
            SetPreviousChoice();
        }

        //1 RIGHT
        if (i == 1)
        {
            setNextChoice();
        }

        //2 UP
        if (i == 2)
        {
            setUpChoice();
        }

        //3 DOWN
        if (i == 3)
        {
            setDownChoice();
        }

        //4 ENTER
        if (i == 4)
        {
            treeTurnCounter++;
            userInterface.TreeCounterUp();
            AddTree();
            Destroy(choiceTile);
            printInnerGrid();
            if (treeTurnCounter < 3)
            {
                PreparePossiblePlaces();
                setNextChoice();
            }
        }
    }

    void CatInputManager(int i)
    {
        
        if (isCatJumping) return;
        if (choiceTile != null) Destroy(choiceTile);
        if (true || catLocation.x == -1 || gridList[catLocation.x][catLocation.y] != 6)
        {
            for (int j = 0; j < gridList.Count; j++)
            {
                for (int k =0; k < gridList[j].Count; ++k)
                {
                    if (gridList[j][k] == 6) catLocation = new Vector2Int(j, k);
                }
        }
        }
        
        //0 LEFT
        if (i == 0)
        {
            isCatJumping = true;
            int new_x = catLocation.x;
            int new_y = catLocation.y;
            int safeCounter = 0;
            while (gridList[new_x][new_y] != 1)
            {
                new_x -= 1;
                if (new_x < 0) (new_y, new_x) = (new_y + 1, gridList.Count-1);
                if (new_y >= gridList[new_x].Count) new_y = 0;
                safeCounter++;
                if (safeCounter > 999)
                {
                    isCatJumping = false;
                    return;
                }
            }
            StartCoroutine(SwapTiles(catLocation, new Vector2Int(new_x, new_y)));
            catLocation = new Vector2Int(new_x, new_y);
        }

        //1 RIGHT
        if (i == 1)
        {
            isCatJumping = true;
            int new_x = catLocation.x;
            int new_y = catLocation.y;
            int safeCounter = 0;
            while (gridList[new_x][new_y] != 1)
            {
                new_x += 1;
                if (new_x >= gridList.Count) (new_y, new_x) = (new_y - 1, 0);
                if (new_y < 0) new_y = gridList[new_x].Count-1;
                safeCounter++;
                if (safeCounter > 999)
                {
                    isCatJumping = false;
                    return;
                }
            }
            
            StartCoroutine(SwapTiles(catLocation, new Vector2Int(new_x,new_y)));
            catLocation = new Vector2Int(new_x, new_y);
        }

        //2 UP
        if (i == 2)
        {
            isCatJumping = true;
            int new_x = catLocation.x;
            int new_y = catLocation.y;
            int safeCounter = 0;
            while (gridList[new_x][new_y] != 1)
            {
                new_y += 1;
                if (new_y >= gridList.Count) new_y = 0;
                safeCounter++;
                if (safeCounter > maxDimension)
                {
                    isCatJumping = false;
                    return;
                }

            }

            StartCoroutine(SwapTiles(catLocation, new Vector2Int(new_x, new_y)));
            catLocation = new Vector2Int(new_x, new_y);
        }

        //3 DOWN
        if (i == 3)
        {
            isCatJumping = true;
            int new_x = catLocation.x;
            int new_y = catLocation.y;
            int safeCounter = 0;
            while (gridList[new_x][new_y] != 1)
            {
                new_y -= 1;
                if (new_y <0) new_y = gridList[new_x].Count - 1;
                safeCounter++;
                if (safeCounter > maxDimension)
                {
                    isCatJumping = false;
                    return;
                }
            }

            StartCoroutine(SwapTiles(catLocation, new Vector2Int(new_x, new_y)));
            catLocation = new Vector2Int(new_x, new_y);
        }

        //4 ENTER
        if (i == 4)
        {
            userInterface.ShowNextRoundButton();
            userInterface.HideArrows();
            userInterface.HideYouCanMoveCat();
            movingCat = false;
        }
    }

    IEnumerator SwapTiles(Vector2Int location1, Vector2Int location2)
    {
        yield return null;
        (gridList[location1.x][location1.y], gridList[location2.x][location2.y]) = (gridList[location2.x][location2.y], gridList[location1.x][location1.y]);
        GameObject tile1 = gridList2[location1.x][location1.y];
        GameObject tile2 = gridList2[location2.x][location2.y];

        float counter = 0;
        Vector3 tile1pos = tile1.transform.position;
        Vector3 tile2pos = tile2.transform.position;
        Quaternion tile1rotation = tile1.transform.rotation;
        bool tile2teleported = false;
        while (counter < flipTime)
        {
            tile1.transform.position = Vector3.Lerp(tile1pos, tile2pos + new Vector3(0, 0.05f, 0), counter/flipTime) + new Vector3(0,Mathf.Sin(Mathf.PI*(counter/flipTime)),0) * flipHeight;
           // tile1.transform.rotation = Quaternion.Lerp(tile1rotation,tile1rotation * Quaternion.Euler(360,0,0),counter/flipTime);
            if (counter > flipTime / 2 && !tile2teleported) 
            {
                tile2teleported = true;
                tile2.transform.position = tile1pos + new Vector3(0,0.05f,0);
            } 
            counter += Time.deltaTime;
            yield return null;
        }
        tile1.transform.position = tile2pos;
        (gridList2[location1.x][location1.y], gridList2[location2.x][location2.y]) = (gridList2[location2.x][location2.y], gridList2[location1.x][location1.y]);
        tile1.transform.rotation = tile1rotation;
        isCatJumping = false;

    }

    IEnumerator DisappearTree(GameObject tree)
    {
        Vector3 startScale = tree.transform.localScale;
        float counter = 0;
        float goalTime = 0.8f;
        while(counter < goalTime)
        {
            yield return null;
            tree.transform.localScale = Vector3.Lerp(startScale, new Vector3(), counter / goalTime);
            counter += Time.deltaTime;
        }
        Destroy(tree.gameObject);
    }

    IEnumerator FlipCatTileOld()
    {
        catTile.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
        catTile.GetComponent<Rigidbody>().AddForce(Vector3.up * flipForce);
        catTile.GetComponent<Rigidbody>().AddTorque(Vector3.left * rotateForce);


        yield return new WaitForSeconds(0.5f);
        catTile.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


    }

    public IEnumerator FlipCatTile()
    {
        Vector3 oldPos = catTile.transform.position;
        Quaternion startRotation = catTile.transform.rotation;
        Quaternion endRotation = catTile.transform.rotation * Quaternion.Euler(180, 0, 0);
        float counter = 0;
        while (counter < flipTime / 2)
        {
            catTile.transform.position = Vector3.Lerp(oldPos, oldPos + new Vector3(0, flipHeight, 0), counter / (flipTime / 2));
            catTile.transform.rotation = Quaternion.Lerp(startRotation, endRotation, counter / flipTime);
            counter += Time.deltaTime;
            yield return null;
        }
        Vector3 newPos = catTile.transform.position;
        while (counter < flipTime)
        {
            catTile.transform.position = Vector3.Lerp(oldPos + new Vector3(0, flipHeight, 0), oldPos, (counter - flipTime / 2) / (flipTime / 2));
            catTile.transform.rotation = Quaternion.Lerp(startRotation, endRotation, counter / flipTime);
            counter += Time.deltaTime;
            yield return null;
        }
        catTile.transform.rotation = endRotation;
        catTile.transform.position = oldPos;
        //userInterface.flipCat();

    }

    public void AddNewShape(int shapeType, int wispType)
    {
        Vector2 catPosition = FindCatTile();
        Vector3 catTilePosition = gridList2[(int)catPosition.x][(int)catPosition.y].transform.position;
        Vector3 shapePosition = catTilePosition;
        GameObject newShape = Instantiate(shapePrefab, shapePosition, Quaternion.identity);
        Shape shapeScript = newShape.GetComponent<Shape>();
        shapeScript.userInterface = userInterface;
        shapeScript.cameraMover = cameraMover;
        shapeScript.gridManager = this;
        shapeScript.gameManager = gameManager;
        shapeScript.startPosition = catPosition;
        shapeScript.GenerateFirstTime(shapeType, wispType);
        currentShape = newShape;
        Destroy(choiceTile);
        print("FINISH");
    }

    public void TreeTurn()
    {
        treeTurnCounter = 0;
        PreparePossiblePlaces();
        setNextChoice();
    }

    public Vector2 GetCatPosition()
    {
        for (int i = 0; i < gridList2.Count; i++) {
            for (int j = 0; j < gridList2[i].Count; j++)
            {
                if (gridList2[i][j] == catTile) return new Vector2(i,j);
            }
                
        }
        return new Vector2();
    }

    public int GetTileType(int x, int y)
    {
        return gridList[x][y];
    }

    void Update()
    {
        if (treeTurnCounter >= 3) return;
        if (currentShape != null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ButtonInputManager(0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ButtonInputManager(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ButtonInputManager(2);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ButtonInputManager(3);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ButtonInputManager(4);
            
        }



    }
}
