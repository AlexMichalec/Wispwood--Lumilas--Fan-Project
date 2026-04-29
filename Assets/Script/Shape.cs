using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Rendering;
using TMPro;

public class Shape : MonoBehaviour
{
    //Prefabs
    public GameObject choiceTilePrefab;
    public GameObject[] tilePrefabs;
    public GameObject badChoiceTilePrefab;


    public float distanceBetweenTiles = 0.5f;
    public float spawnHeight = 0.1f;
    public float spawnHeightChoice = 0.05f;
    
    private List<List<Vector2>> shapesList = new List<List<Vector2>>();
    private List<Vector2> myShape = new List<Vector2>();
    private List<GameObject> tileList = new List<GameObject > ();

    public bool isFinal = false;
    private int ghostIndex = 0;
    private int ghostTypeIndex;
    public bool testing = false;
    public Vector2 startPosition = Vector2.zero;
    private bool canBePlacedWhole = false;

    //Navigation
    public GridManager gridManager;
    public UI userInterface;
    public MoveCamera cameraMover;
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeShapesList();
        //GenerateRandom();

        
        
    }

    void InitializeShapesList()
    {
        shapesList.Clear();
        //Shapes:
        //[][]
        List<Vector2> shape1 = new List<Vector2> { new Vector2(0,0), new Vector2(0,1)};
        shapesList.Add(shape1);

        //[][][]
        List<Vector2> shape2 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) };
        shapesList.Add(shape2);

        //[][][][]
        List<Vector2> shape3 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3) };
        shapesList.Add(shape3);

        //[][]
        //[][]
        List<Vector2> shape4 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
        shapesList.Add(shape4);

        //[]
        //[][]
        List<Vector2> shape5 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0)};
        shapesList.Add(shape5);


        //  [][]
        //[][]
        List<Vector2> shape6 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) };
        shapesList.Add(shape6);

        //  []
        //[][][]
        List<Vector2> shape7 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 1) };
        shapesList.Add(shape7);

        //[]
        //[][][]
        List<Vector2> shape8 = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 0) };
        shapesList.Add(shape8);

    }
    public void GenerateRandom()
    {
        ghostTypeIndex = Random.Range(0, 4);
        ghostIndex = 0;
        GenerateFirstTime(Random.Range(0,shapesList.Count), ghostTypeIndex);
    }

   public  void GenerateFirstTime(int shapeIndex, int WispType)
    {
        InitializeShapesList();
        myShape = shapesList[shapeIndex];
        ghostTypeIndex = WispType;
        Generate();
    }

    void Generate()
    {
        Clear();
        canBePlacedWhole = true;
        for (int i = 0; i < myShape.Count; i++)
        {
            
            GameObject chosenPrefab = choiceTilePrefab;
            if (isFinal) chosenPrefab = NextTile();
            if (i == ghostIndex) chosenPrefab = GetGhostTile();
            if (!CanBePlacedHere(myShape[i]) || !WitchCondtion(i, myShape[i]))
            {
                chosenPrefab = badChoiceTilePrefab;
                canBePlacedWhole = false;
            }
                
            

            Vector3 tilePosition = new Vector3(transform.position.x + myShape[i].x * distanceBetweenTiles,spawnHeightChoice, transform.position.z + myShape[i].y * distanceBetweenTiles);
            if (isFinal) tilePosition += new Vector3(0, spawnHeight, 0);

            Quaternion tileRotation = Quaternion.Euler(0,180,0);
            if (isFinal && !(i == ghostIndex)) tileRotation = Quaternion.Euler(0, 0, 180);

            GameObject newTile = Instantiate(chosenPrefab, tilePosition, tileRotation);
            tileList.Add(newTile);

            //print(myShape[i]);
        }
        
    }

    bool CanBePlacedHere(Vector2 location)
    {
        location += startPosition;
        if (DoesntTouch()) return false;
        if (TooFar(location)) return false;
        if (IsNotEmpty(location)) return false;
        return true;
    }

    bool TooFar(Vector2 location)
    {
        int maxDimension = gridManager.maxDimension;
        Vector2 gridSize = gridManager.GetSize();
        if (location.x < 0 && gridSize.x - location.x > maxDimension) return true;
        if (location.y < 0 && gridSize.y - location.y > maxDimension) return true;
        if (location.x+1 > maxDimension) return true;
        if (location.y+1 > maxDimension) return true;
        return false;
    }

    bool IsNotEmpty(Vector2 location)
    {
        if (location.x < 0 || location.y < 0) return false;
        if (location.x >= gridManager.GetSize().x || location.y >= gridManager.GetSize().y) return false;
        return !gridManager.isEmpty(location);
    }

    bool DoesntTouch()
    {
        List<Vector2> neighbourLocationModifier = new List<Vector2> { new Vector2(-1,0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1), };
        for (int i = 0; i < myShape.Count; ++i)
        {
            for (int j = 0; j< 4; ++j)
            {
                Vector2 location = myShape[i] + startPosition + neighbourLocationModifier[j];
                if (location.x < 0 || location.y < 0) continue;
                if (location.x >= gridManager.GetSize().x || location.y >= gridManager.GetSize().y) continue;
                if (!gridManager.isEmpty(location)) return false;
            }
            
        }
        return true;
    }

    bool WitchCondtion(int tileIndex, Vector2 location)
    {
        if (tileIndex != ghostIndex) return true;
        if (ghostTypeIndex != 2) return true;

        Vector2 witchLocation = location + startPosition;
        Vector2 catLocation = gridManager.GetCatPosition();
        int methodIndex = gridManager.myScore.witchScoreMethodIndex;

        //Only Debug (ignore Witch condition)
        if (methodIndex == 0) return true;

        //"Każda nowa wiedźma musi zostać umieszczona na linii ukośnej z kotem."
        if (methodIndex == 1) return catLocation.x - catLocation.y == witchLocation.x - witchLocation.y || 
                catLocation.x + catLocation.y == witchLocation.x + witchLocation.y;

        //"Każda nowa wiedźma musi zostać umieszczona w rzędzie lub kolumnie z kotem" 
        if (methodIndex == 2) return catLocation.x == witchLocation.x || catLocation.y == witchLocation.y;

        //Każda nowa wiedźma musi zostać umieszczona na jednym z pól wokół kota.
        if (methodIndex == 3) return Mathf.Abs(catLocation.x - witchLocation.x) <= 1 && Mathf.Abs(catLocation.y - witchLocation.y) <= 1;

        //"Każda nowa wiedźma musi łączyć się skośnie z kotem, bezpośrednio lub przez inne wiedźmy"
        if (methodIndex == 4)
        {
            Queue<Vector2> lazyQ = new Queue<Vector2>();
            List<Vector2> visited = new List<Vector2>();
            lazyQ.Enqueue(witchLocation);
            Vector2[] helpArray = { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) };
            while (lazyQ.Count > 0)
            {
                
                for (int i = 0; i < helpArray.Length; i++)
                {
                    int tempX = (int) (lazyQ.Peek().x + helpArray[i].x);
                    int tempY = (int)(lazyQ.Peek().y + helpArray[i].y);
                    Vector2 tVector = new Vector2(tempX, tempY);
                    if (catLocation == tVector) return true;
                    if (visited.Contains(tVector)) continue;
                    if (tempX < 0 || tempY < 0) continue;
                    if (tempX >= gridManager.GetSize().x) continue;
                    if (tempY >= gridManager.GetSize().y) continue;
                    if (gridManager.GetTileType(tempX, tempY) == 4) lazyQ.Enqueue(tVector);
                }
                visited.Add(lazyQ.Dequeue());

            }
            return false;
        }

        //"Każda nowa wiedźma musi być umieszczona dokładnie dwa pola od kota."
        if (methodIndex == 5) return (Mathf.Abs(catLocation.x - witchLocation.x) == 2 && Mathf.Abs(catLocation.y - witchLocation.y) <= 2) || 
                (Mathf.Abs(catLocation.y - witchLocation.y) == 2 && Mathf.Abs(catLocation.x - witchLocation.x) <= 2);


        return false;
    }

    GameObject NextTile()
    {
        return tilePrefabs[Random.Range(0, tilePrefabs.Count())];
    }

    GameObject GetGhostTile()
    {
        return tilePrefabs[ghostTypeIndex];
    }

    void Clear()
    {
        for (int i = 0; i < tileList.Count; ++i)
        {
            Destroy(tileList[i]);
        }
        tileList.Clear();
    }

    void RotateRight()
    {
        for (int i = 0; i < myShape.Count; ++i)
        {
            float x = myShape[i].x;
            myShape[i] = new Vector2(myShape[i].y, -myShape[i].x);
        }
        Generate();
    }

    void RotateLeft()
    {
        for (int i = 0; i < myShape.Count; ++i)
        {
            float x = myShape[i].x;
            myShape[i] = new Vector2(-myShape[i].y, myShape[i].x);
        }
        Generate();
    }

    void FlipShape()
    {
        for (int i = 0; i < myShape.Count; ++i)
        {
            float x = myShape[i].x;
            myShape[i] = new Vector2(-myShape[i].x, myShape[i].y);
        }
        Generate();
    }

    void ChangeGhostIndex()
    {
        ghostIndex = (ghostIndex + 1) % myShape.Count;
        Generate();
    }

    void MoveUp()
    {
        if (!CanMove(startPosition + new Vector2(0, 1))) return;
        startPosition += new Vector2(0, 1);
        transform.position += new Vector3(0,0,distanceBetweenTiles);
        Generate();
    }

    void MoveDown()
    {
        if (!CanMove(startPosition + new Vector2(0, -1))) return;
        startPosition += new Vector2(0, -1);
        transform.position += new Vector3(0, 0, -distanceBetweenTiles);
        Generate();
    }

    void MoveLeft()
    {
        if (!CanMove(startPosition + new Vector2(-1, 0))) return;
        startPosition += new Vector2(-1, 0);
        transform.position += new Vector3(-distanceBetweenTiles, 0, 0);
        Generate();
    }

    void MoveRight()
    {
        if (!CanMove(startPosition + new Vector2(1, 0))) return;
        startPosition += new Vector2(1, 0);
        transform.position += new Vector3(distanceBetweenTiles, 0, 0);
        Generate();
    }

    bool CanMove(Vector2 newPosition)
    {
        
        float minX = 99;
        float maxX = -99;
        float minY = 99;
        float maxY = -99;
        for (int i = 0; i < myShape.Count; ++i)
        {
            if (myShape[i].x + newPosition.x < minX) minX = (int)myShape[i].x + (int)startPosition.x;
            if (myShape[i].y + newPosition.y < minY) minY = (int)myShape[i].y + (int)startPosition.y;
            if (myShape[i].x + newPosition.x > maxX) maxX = (int)myShape[i].x + (int)startPosition.x +1;
            if (myShape[i].y + newPosition.y > maxY) maxY = (int)myShape[i].y + (int)startPosition.y +1;
        }

        //print("minX" + minX + " minY " + minY + " maxX " + maxX + " maxY " + maxY);
        //if (maxX - minX > gridManager.maxDimension || maxY - minY > gridManager.maxDimension) return false;
        //Nie mam na to teraz pomysłu xd
        return true;

    }

    void MakeFinal()
    {
        isFinal = true;
        Generate();
        Vector2 gridSize = gridManager.GetSize();
        int minX = 99;
        int minY = 99;
        int maxX = -99;
        int maxY = -99;
        for (int i = 0; i < myShape.Count; i++)
        {
            if (myShape[i].x + startPosition.x < minX) minX = (int)myShape[i].x + (int)startPosition.x;
            if (myShape[i].y + startPosition.y < minY) minY = (int)myShape[i].y + (int)startPosition.y;
            if (myShape[i].x + startPosition.x > maxX) maxX = (int)myShape[i].x + (int)startPosition.x;
            if (myShape[i].y + startPosition.y > maxY) maxY = (int)myShape[i].y + (int)startPosition.y;
        }
       // print("minX" + minX + " minY " + minY + " maxX " + maxX + " maxY " + maxY);
        maxX = maxX - (int)gridSize.x + 1;
        if (maxX < 0) maxX = 0;
        maxY = maxY - (int)gridSize.y + 1;
        if (maxY < 0) maxY = 0;
        if (minX > 0) minX = 0;
        if (minY > 0) minY = 0;
       // print("StartPostion: " + startPosition);
       // print("minX" + minX + " minY " + minY + " maxX " + maxX + " maxY " + maxY);
        gridManager.ExtendGrid(minX, minY);
        
        gridManager.ExtendGrid(maxX, maxY);
        gridManager.printInnerGrid();
        //return;
        for (int i = 0; i < myShape.Count; i++)
        {
            if (minX < 0) myShape[i] -= new Vector2(minX, 0);
            if (minY < 0) myShape[i] -= new Vector2(0, minY);
            int tileTypeIndex = 1;
            if(i==ghostIndex) tileTypeIndex = 2 + ghostTypeIndex;
            gridManager.AddTileFromShape((int)myShape[i].x + (int)startPosition.x, 
                (int)myShape[i].y + (int)startPosition.y, 
                tileList[i], tileTypeIndex);
        }

        //gridManager.printInnerGrid();
        gameManager.FinalizeChoice();
        userInterface.HideArrows();
        if (gridManager.IsFull() || gridManager.isLastTurn)
        {
            gameManager.FinishRound();
            Destroy(gameObject);
        }
        else StartCoroutine(WaitMoveCamera());
        
    }

    IEnumerator WaitMoveCamera()
    {
        yield return new WaitForSeconds(0.8f);
        gameManager.NextPlayer();
        //cameraMover.changePosition();
        Destroy(gameObject);
    }

    public void ButtonManager(int i )
    {

        //0 LEFT
        if (i == 0)
        {
            MoveLeft();
        }

        //1 RIGHT
        if (i == 1)
        {
            MoveRight();
        }

        //2 UP
        if (i == 2)
        {
            MoveUp();
        }

        //3 DOWN
        if (i == 3)
        {
            MoveDown();
        }

        //4 ENTER
        if (i == 4)
        {
            if (canBePlacedWhole) MakeFinal();
        }

        //5 ROTATE LEFT
        if (i == 5)
        {
            RotateLeft();
        }

        //6 ROTATE RIGHT
        if (i == 6)
        {
            RotateRight();
        }

        //7 FLIP HORIZONTALLY
        if (i == 7)
        {
            FlipShape();
        }

        //8 CHANGE WISP POSITION
        if (i == 8)
        {
            ChangeGhostIndex();
        }

        //9 UNDO
        if (i == 9)
        {
            gameManager.UndoChoice();
            userInterface.HideArrows();
            cameraMover.changePosition();
            for (int j = 0; j < tileList.Count; ++j)
            {
                Destroy(tileList[j]);
            }
            Destroy(gameObject);
        }
    }


 

        // Update is called once per frame
    void Update()
    {
        if (isFinal) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ButtonManager(6);

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ButtonManager(5);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ButtonManager(7);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ButtonManager(4);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ButtonManager(8);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ButtonManager(2);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ButtonManager(3);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ButtonManager(0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ButtonManager(1);
        }


    }
}
