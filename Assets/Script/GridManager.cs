using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine.Rendering;

public class GridManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    [SerializeField]
    private GameObject catTile;
    public int maxDimension = 4;
    public float distanceBetweenTiles = 0.4f;
    public float spawnHeight = 0.1f;
    private List<List<int>> gridList = new List<List<int>>();
    private List<List<GameObject>> gridList2 = new List<List<GameObject>>();
    public bool useUp = true;
    public bool useDown = true;
    public bool useLeft = true;
    public bool useRight = true;
    private List<Vector2> possiblePlacesToAddTile = new List<Vector2>();
    public GameObject choiceTilePrefab;
    private GameObject choiceTile;
    public int choiceIndex = 0;
    private Vector2 nearChoiceLocation;
    public bool useMouse = false;
    public Vector3 MouseePos;
    private float moveCenter = 0.02f;
    public GameObject shapePrefab;
    private List<Shape> shapes = new List<Shape>();
    private GameObject currentShape;
    

    // 0 - puste, 1 - drzewo, 2 - dynia, 3 - serce, 4 - wiedżma, 5 - duszek, 6 - kot
    void Start()
    {
        InitializeGrid();
        moveCenter = distanceBetweenTiles / 20.0f;
        Debug.Log("MC " + moveCenter);
        StartCoroutine(lateUpdateChoiceTile());
        
    }

    IEnumerator lateUpdateChoiceTile()
    {
        yield return new WaitForSeconds(0.5f);
        updateChoiceTile();

        
    }

    void InitializeGrid()
    {
        gridList.Add(new List<int> { 6 });
        Vector3 catPosition = new Vector3(transform.position.x,spawnHeight,transform.position.z);
        gridList2.Add(new List<GameObject> { Instantiate(catTile, catPosition, new Quaternion(0, 180, 0, 0) )});
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

    void setPreviousChoice()
    {
        if (IsFull()) return;
        choiceIndex = choiceIndex -1;
        if (choiceIndex < 0) choiceIndex = possiblePlacesToAddTile.Count-1;
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
        if (pt_y<0)
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
            GameObject otherTile = gridList2[pt_x-1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x + distanceBetweenTiles,
                0.01f,otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x - 1, pt_y);
            return;
        }
        //DOWN
        if (pt_x < gridList.Count-1 && gridList[pt_x + 1][pt_y] > 0)
        {
            GameObject otherTile = gridList2[pt_x + 1][pt_y];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x - distanceBetweenTiles,
                0.01f, otherTile.transform.position.z);
            nearChoiceLocation = new Vector2(pt_x + 1, pt_y);
            return;
        }
        //LEFT
        if (pt_y > 0 && gridList[pt_x][pt_y-1] > 0)
        {
            GameObject otherTile = gridList2[pt_x][pt_y-1];
            choiceTile.transform.position = new Vector3(otherTile.transform.position.x,
                0.01f, otherTile.transform.position.z + distanceBetweenTiles);
            nearChoiceLocation = new Vector2(pt_x, pt_y - 1);
            return;
        }
        //RIGHT
        if (pt_y < gridList[0].Count && gridList[pt_x][pt_y + 1] > 0)
        {
            GameObject otherTile = gridList2[pt_x][pt_y+1];
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
                if (CanAddLeft(i,j) && !possiblePlacesToAddTile.Contains(new Vector2(i,j-1))) 
                    possiblePlacesToAddTile.Add(new Vector2(i,j-1));
                if (CanAddRight(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i, j + 1)))
                    possiblePlacesToAddTile.Add(new Vector2(i, j + 1));
                if (CanAddUp(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i-1, j)))
                    possiblePlacesToAddTile.Add(new Vector2(i-1, j));
                if (CanAddDown(i, j) && !possiblePlacesToAddTile.Contains(new Vector2(i+1, j)))
                    possiblePlacesToAddTile.Add(new Vector2(i+1, j));

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

    bool IsFull()
    {
        if (gridList.Count < maxDimension || gridList[0].Count < maxDimension) return false;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j< gridList[i].Count; ++j)
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
        if (tileIndex<0) tileIndex = Random.Range(0, 4);
        int gridTileIndex = 1;
        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);
        newTile.transform.rotation = new Quaternion(0, 0, 180, 0);
        Vector2 chosenPlace = possiblePlacesToAddTile[choiceIndex];
        AddNewTile(chosenPlace, newTile, gridTileIndex, nearChoiceLocation);

    }

    void addRandomTile()
    {
        if (IsFull()) return;
        int tileIndex = Random.Range(0,4);
        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);
        bool isTree = Random.value > 0.5;
        Vector2 usedTile = new Vector2();
        int gridTileIndex = 1;
        if (isTree) newTile.transform.rotation = new Quaternion(0, 0, 180, 0);
        else {
            newTile.transform.Rotate(Vector3.up, 180);
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
            if (useLeft && CanAddLeft(x, y)) potentialPlaces.Add(new Vector2(x, y-1));
            if (useRight && CanAddRight(x, y)) potentialPlaces.Add(new Vector2(x, y+1));
            if (useUp && CanAddUp(x, y)) potentialPlaces.Add(new Vector2(x-1, y));
            if (useDown && CanAddDown(x, y)) potentialPlaces.Add(new Vector2(x+1, y));
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
            usedTile = new Vector2(usedTile.x+1, usedTile.y);
            StartCoroutine(CenterGrid(moveCenter,0));
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
            chosenPlace = new Vector2(chosenPlace.x, chosenPlace.y+1);
            usedTile = new Vector2(usedTile.x, usedTile.y+1);
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
        return (y < gridList[0].Count - 1 && gridList[x][y+1] == 0) || (y == gridList[0].Count-1) && gridList[0].Count < maxDimension;
    }

    bool CanAddDown(int x, int y)
    {
        return (x < gridList.Count-1 && gridList[x+1][y] == 0) || (x == gridList.Count-1) && gridList.Count < maxDimension;
    }

    IEnumerator CenterGrid(float x, float y) 
    {
        yield return new WaitForSeconds(0.2f);
        for (int t = 0; t < 10; t++)
        {
            for(int i = 0; i < gridList2.Count; i++)
            {
                for(int j=0; j < gridList2[i].Count; j++)
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
        StartCoroutine(CenterGrid(-x * moveCenter, -y *moveCenter));
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
        while (x >0)
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
        while (y >0)
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentShape == null)
            //if (shapes.Count == 0 || shapes[shapes.Count-1].isFinal)
            {
                Vector2 catPosition = FindCatTile();
                Vector3 catTilePosition = gridList2[(int)catPosition.x][(int)catPosition.y].transform.position;
                Vector3 shapePosition = catTilePosition;
                GameObject newShape = Instantiate(shapePrefab, shapePosition, Quaternion.identity);
                newShape.GetComponent<Shape>().startPosition = catPosition;
                currentShape = newShape;
                shapes.Add(newShape.GetComponent<Shape>());
                Destroy(choiceTile);
            }

            
            
        }
        if (currentShape!= null) return;
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(choiceTile);
            choiceIndex = 0;
            addRandomTile();
        }
        if (useMouse)
        {
            MouseePos = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            PreparePossiblePlaces();
            string result = "Result: \n";
            for (int i = 0; i < possiblePlacesToAddTile.Count; i++) {
                result += "(" + possiblePlacesToAddTile[i].x + ", " + possiblePlacesToAddTile[i].y + "), ";
                   
            }
            Debug.Log(result);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreparePossiblePlaces();
            setPreviousChoice();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PreparePossiblePlaces();
            setNextChoice();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PreparePossiblePlaces();
            setUpChoice();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PreparePossiblePlaces();
            setDownChoice();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddTree();
            Destroy(choiceTile);
            printInnerGrid();
            
        }
    }
}
