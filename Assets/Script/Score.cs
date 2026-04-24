using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering;

public class Score : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [HideInInspector]
    public List<List<int>> gridList = new List<List<int>>();
    [Header("Scoring Methods")]
    [Tooltip("0 means don't add it to the Score")]
    [Range(0, 5)]
    public int pumpkinsScoreMethodIndex = 1;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0,5)]
    public int heartScoreMethodIndex = 1;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0, 5)]
    public int witchScoreMethodIndex = 1;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0, 6)]
    public int orbScoreMethodIndex = 1;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0, 5)]
    public int treeScoreMethodIndex = 1;
    [Header("Navigation")]
    public UI userInterface;
    [HideInInspector]
    public int myScore = 0;

    //DEBUG:
    private float treesRatio = 0.5f;
    void Start()
    {
        if (gridList.Count == 0) GetRandomGrid();
        PrintGrid();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScoreMethod(int scoreType, int methodIndex)
    {
        if (scoreType == 0) treeScoreMethodIndex = methodIndex;
        if (scoreType == 1) pumpkinsScoreMethodIndex = methodIndex;
        if (scoreType == 2) heartScoreMethodIndex = methodIndex;
        if (scoreType == 3) witchScoreMethodIndex = methodIndex;
        if (scoreType == 4) orbScoreMethodIndex = methodIndex;
    }

    public static string [] GetInfoScoreMethods(int wispType, int methodIndex)
    {
        if (wispType == 2) 
        {
            //PUMPKINS
            if (methodIndex == 0) return new string[] { "Empty Method","No points (only Debug)" };
            if (methodIndex == 1) return new string[] { "Dynioku", "Policz Dynie znajdujące się w rzędzie i kolumnie bez innych Dyń.", "(1,4), (2,9), (3,15), (4,22) (5,30), (6,40)" };
            if (methodIndex == 2) return new string[] { "Dyniowa Latarnia", "Policz Dynie wokół których nie ma innych Dyni.", "(1,4), (2,9), (3,15), (4,22), (5,30) + 8 za każdą kolejną" };
            if (methodIndex == 3) return new string[] { "Dynia z parą", "Policz pary Dyń, obok których nie ma innych Dyni.", "(1,11), (2,23), (3,37) + 14 za każdą kolejną parę" };
            if (methodIndex == 4) return new string[] { "Bez dyni ani rusz", "Policz grupy duszków z conajmniej jedną Dynią.", "(1,4), (2,9), (3,15), (4,22), (5,30) + 8 za każdą kolejną" };
            if (methodIndex == 5) return new string[] { "Pokrzyżowane plany", "Policz Dynie, które nie leża na linii ukośnej z innymi Dyniami", "(1,4), (2,9), (3,15), (4,22), (5,30), (6, 39)"};
        }
        if (wispType == 3)
        {
            //HEARTS
            if (methodIndex == 0) return new string[] { "Empty Method", "No points (only Debug)" };
            if (methodIndex == 1) return new string[] { "Promień Krzyżowy", "Każde Serce: 1 punkt za każde Drzewo w tym samym rzędzie i tej samej kolumnie." };
            if (methodIndex == 2) return new string[] { "Co z oczu, to z serca", "Każde Serce: 2 punkty za każde Drzewo obok niego." };
            if (methodIndex == 3) return new string[] { "Śreżoga", "Każde Serce: 2 punkty za każde Drzewo poniżej w tej samej kolumnie." };
            if (methodIndex == 4) return new string[] { "Promyk Nadziei", "Kaze Serce: 2 punkty za każde Drzewo na jednej z ukośnych linii tego Serca." };
            if (methodIndex == 5) return new string[] { "Przez drzewo do serca", "Każde Serce: 2 punkty za każde Drzewo w najdłuższej nieprzerwanej wychodzącej z niego linii." }; ;
        }
        if (wispType == 4)
        {
            //WITCHES
            if (methodIndex == 0) return new string[] { "Empty Method", "No points (only Debug)" };
            if (methodIndex == 1) return new string[] { "Koci Kącik", "Każda nowa wiedźma musi zostać umieszczona na linii ukośnej z kotem.", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };
            if (methodIndex == 2) return new string[] { "Wlazł kotek na płotek", "Każda nowa wiedźma musi zostać umieszczona w rzędzie lub kolumnie z kotem", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };
            if (methodIndex == 3) return new string[] { "Po nitce do kłębka", "Każda nowa wiedźma musi zostać umieszczona na jednym z pól wokół kota.", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };
            if (methodIndex == 4) return new string[] { "Miaucaby", "Każda nowa wiedźma musi łączyć się skośnie z kotem, bezpośrednio lub przez inne wiedźmy", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };
            if (methodIndex == 5) return new string[] { "Superpozycja", "Każda nowa wiedźma musi być umieszczona dokładnie dwa pola od kota.", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };
            else return new string[] { "Uniwersalna", "18p za każde 3 wiedźmy, 10p/4p za pozostałe 2/1." };

        }

        if (wispType == 5)
        {
            //ORBS
            if (methodIndex == 0) return new string[] { "Empty Method", "No points (only Debug)" };
            if (methodIndex == 1) return new string[] { "Przyciągający blask", "Każdy Ognik: 2 punkty za każdy rodzaj duszka wokół niego." };
            if (methodIndex == 2) return new string[] { "Światełko w tunelu", "Każdy Ognik: 2 punkty za każdy rodzaj duszka w tym rzędzie albo kolumnie (łącznie z tym Ognikiem)" };
            if (methodIndex == 3) return new string[] { "Szukaj igły w stogu światła", "Którego rodzaju masz najmniej albo wcale? Każdy Ognik jest wart tyle punktów", "Ognik: 7, Dynia: 5, Wiedźma: 4, Serce: 3" };
            if (methodIndex == 4) return new string[] { "Energia - synergia", "Każdy Ognik jest wart tyle punktów co najmniej wartościowy rodzaj duszka obok niego.", "Ognik: 4, Dynia: 5, Wiedźma: 6, Serce: 7" };
            if (methodIndex == 5) return new string[] { "W stronę światła", "Każda grupa duszków z conajmniej 1 Ognikiem: 2 punkty za każdy rodzaj duszka w tej grupie." };
            if (methodIndex == 6) return new string[] { "Płochliwy promyk", "Dla każdergo Ognika: Wybierz 1 grupę duszków, która dotyka tylko narożnika tego Ognika. 2 punkty za każdy rodzaj duszka w tej grupie." }; ;
        }

        if (wispType == 1)
        {
            //TREES
            if (methodIndex == 0) return new string[] { "Empty Method", "No points (only Debug)" };
            if (methodIndex == 1) return new string[] { "Co trzy drzewa, to nie jedno", "2 punkty za każdy rząd i każdą kolumnę, w którch są conajmniej 3 Drzewa." };
            if (methodIndex == 2) return new string[] { "Ciągnie drzewo do lasu", "# punkty za każde Drzewo, obok którego są conajmniej 3 drzewa." };
            if (methodIndex == 3) return new string[] { "Jeden, by wszystkie posadzić", "1 punkt za każde Drzewo w największej grupie Drzew." };
            if (methodIndex == 4) return new string[] { "Im dalej w las...", "2 punkty za każde Drzewo w drugiej co do wielkości grupie Drzew." };
            if (methodIndex == 5) return new string[] { "Jak szyszka w kompot", "4 punkty za każdą grupę składającą się z conajmniej 3 Drzew." }; ;
        }

        return new string[] { "Error", "Wrong arguments" };
        
    }

    public void SumUpScore()
    {
        int result = 0;
        //ADD PUMPKIN SCORE
        result += ScorePumpkins();
        //ADD HEARTS SCORE
        result += ScoreHearts();
        //ADD WITCH SCORE
        result += ScoreWitches();
        //ADD ORB SCORE
        result += ScoreOrbs();
        //ADD TREES SCORE
        result += ScoreTrees();
        //If forest full add 2/4/6 points
        result += ScoreFullForest();
        myScore += result;
        userInterface.UpdateScore(myScore);
        //TYMCZASOWO TU:
        ShowDetailScore();
    }

    void ShowDetailScore()
    {
        int[] scoreArray = new int[] {0,0,0,0,0,0};
        //ADD PUMPKIN SCORE
        scoreArray[1]= ScorePumpkins();
        //ADD HEARTS SCORE
        scoreArray[2] = ScoreHearts();
        //ADD WITCH SCORE
        scoreArray[3] = ScoreWitches();
        //ADD ORB SCORE
        scoreArray[4] = ScoreOrbs();
        //ADD TREES SCORE
        scoreArray[0] = ScoreTrees();
        //If forest full add 2/4/6 points
        scoreArray[5] = ScoreFullForest();
        userInterface.UpdateDetailedScore(scoreArray);
    }

    int ScorePumpkins()
    {
        int subScore = 0;
        if (pumpkinsScoreMethodIndex == 1) subScore = ScorePumpkinsSudoku();
        if (pumpkinsScoreMethodIndex == 2) subScore = ScorePumpkinsLantern();
        if (pumpkinsScoreMethodIndex == 3) subScore = ScorePumpkinsPairs();
        if (pumpkinsScoreMethodIndex == 4) subScore = ScorePumpkinsGroups();
        if (pumpkinsScoreMethodIndex == 5) subScore = ScorePumpkinsDiagonal();
        print("Pumpkins score: " + subScore);
        return subScore;
    }

    int ScoreHearts() 
    {
        int subScore = 0;
        if (heartScoreMethodIndex == 1) subScore = ScoreHeartsCross();
        if (heartScoreMethodIndex == 2) subScore = ScoreHeartsAround();
        if (heartScoreMethodIndex == 3) subScore = ScoreHeartsColumnBelow();
        if (heartScoreMethodIndex == 4) subScore = ScoreHeartsRays();
        if (heartScoreMethodIndex == 5) subScore = ScoreHeartsLongestLine();
        print("Hearts score: " + subScore);
        return subScore;

    }
    int ScoreWitches()
    {

        int subScore = 0;
        if (witchScoreMethodIndex != 0) subScore += ScoreWitchesUniversal();
        print("Witches score: " + subScore);
        return subScore;

    }

    int ScoreOrbs()
    {

        int subScore = 0;
        if (orbScoreMethodIndex == 1) subScore += ScoreOrbsAround();
        if (orbScoreMethodIndex == 2) subScore += ScoreOrbsTunnel();
        if (orbScoreMethodIndex == 3) subScore += ScoreOrbsNeedle();
        if (orbScoreMethodIndex == 4) subScore += ScoreOrbsSynergy();
        if (orbScoreMethodIndex == 5) subScore += ScoreOrbsGroups();
        if (orbScoreMethodIndex == 6) subScore += ScoreOrbsGroupsCorners();
        print("Orbs score: " + subScore);
        return subScore;
    }

    int ScoreTrees() 
    {
        int subScore = 0;
        if (treeScoreMethodIndex == 1) subScore += ScoreTreesRowColumn();
        if (treeScoreMethodIndex == 2) subScore += ScoreTreesAround();
        if (treeScoreMethodIndex == 3) subScore += ScoreTreesBiggestGroup();
        if (treeScoreMethodIndex == 4) subScore += ScoreTreesSecondBiggestGroup();
        if (treeScoreMethodIndex == 5) subScore += ScoreTreesGroupsBiggerThanThree();
        print("Trees score: " + subScore);
        return subScore;
    }

    //Dynioku
    int ScorePumpkinsSudoku()
    {
        int result = 0;
        int counter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 2)
                {
                    int tempCounter = 0;
                    for (int k = 0; k < gridList.Count; ++k)
                    {
                        if (gridList[i][k] == 2) tempCounter++;
                        if (gridList[k][j] == 2) tempCounter++;
                    }
                    // 2 because for wold count it twice ^
                    if (tempCounter == 2) counter++;
                }

            }

        }
        int[] pointsArray = {0, 4, 9, 15, 22, 30, 40 };
        result = pointsArray[counter];
        return result;
    }

    //Dyniowa Latarnia
    //Policz Dynie wokół których nie ma innych Dyni.
    //(1,4), (2,9), (3,15), (4,22) (5,30) + 8 za każdą kolejną
    int ScorePumpkinsLantern()
    {
        int result = 0;
        int counter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 2)
                {
                    int tempCounter = 0;
                    for (int k = -1; k <= 1; ++k)
                    {
                        if ((i + k) < 0) continue;
                        if ((i + k) >= gridList.Count) continue;
                        if ((j - 1) >= 0 && gridList[i + k][j - 1] == 2) tempCounter++;
                        if (gridList[i + k][j] == 2) tempCounter++;
                        if ((j + 1) < gridList.Count && gridList[i + k][j + 1] == 2) tempCounter++;
                    }
                    if (tempCounter == 1) counter++;
                }

            }

        }
        int[] pointsArray = { 0, 4, 9, 15, 22, 30};
        if (counter <= 5) result = pointsArray[counter];
        else result = 30 + 8 * (counter - 5);
        return result;
    }

    //Dynia z parą
    //Policz pary Dyni obok których nie ma innych Dyni
    //(1,11), (2,23), (3,37) + 14 za każdą kolejną parę
    int ScorePumpkinsPairs()
    {
        int result = 0;
        int counter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 1)
                {
                    if ((i+1<gridList.Count && gridList[i + 1][j] == 1))
                    {
                        //[.][X][.]
                        //[X][D][X]
                        //[X][D][X]
                        //[.][X][.]
                        if ((i - 1 >= 0) && gridList[i - 1][j] == 1) continue;
                        if ((j - 1 >= 0) && gridList[i][j - 1] == 1) continue;
                        if ((j + 1 < gridList[i].Count) && gridList[i][j + 1] == 1) continue;
                        if ((j - 1 >= 0) && gridList[i + 1][j - 1] == 1) continue;
                        if ((j + 1 < gridList[i].Count) && gridList[i + 1][j + 1] == 1) continue;
                        if ((i + 2 < gridList.Count) && gridList[i + 2][j] == 1) continue;
                        counter++;
                    }
                    else if ((j + 1 < gridList[i].Count && gridList[i][j + 1] == 1))
                    {
                        //[.][X][X][.]
                        //[X][D][D][X]
                        //[.][X][X][.]
                        if ((i - 1 >= 0) && gridList[i - 1][j] == 1) continue;
                        if ((i - 1 >= 0) && gridList[i - 1][j + 1] == 1) continue;
                        if ((j - 1 >= 0) && gridList[i][j - 1] == 1) continue;
                        if ((j + 2 < gridList[i].Count) && gridList[i][j + 2] == 1) continue;
                        if ((i + 1 < gridList.Count) && gridList[i + 1][j] == 1) continue;
                        if ((i + 1 < gridList.Count) && gridList[i + 1][j + 1] == 1) continue;
                        counter++;
                    }
                    
                }

            }

        }
        int[] pointsArray = { 0, 11, 23, 37};
        if (counter <= 3) result = pointsArray[counter];
        else result = 37 + 14 * (counter - 3);
        return result;
    }

    //Bez Dynii ani rusz
    //Policz grupy duszków z conajmniej jedną Dynią
    //1-4, 2-9, 3-15, 4-22, 5-30, +8 za każdą kolejną
    int ScorePumpkinsGroups()
    {
        int result = 0;
        int groupCount = 0;
        int[] scoreArray = { 0, 4, 9, 15, 22, 30 };
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        List<Vector2Int> visited = new List<Vector2Int>();
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j=0; j < gridList[i].Count; ++j)
            {
                if (gridList[i][j] == 2 && !visited.Contains(new Vector2Int(i,j)))
                {
                    groupCount++;
                    queue.Enqueue(new Vector2Int(i, j));
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;

                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }

                        if (x > 0 && IsWisp(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x-1, y));
                        if (y > 0 && IsWisp(x, y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y-1));
                        if (x < gridList.Count - 1 && IsWisp(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count-1 && IsWisp(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                }
            }
        }
        result = scoreArray[Mathf.Min(groupCount, scoreArray.Length - 1)];
        if (groupCount >= scoreArray.Length) result += (groupCount - (scoreArray.Length - 1) * 8);

        return result;
    }

    //Pokrzyżowane plany
    //Policz Dynie, które nie leżą na ukośnej linii z żadną inną Dynią
    //1-4, 2-9, 3-15, 4-22, 5-30, +8 za każdą kolejną
    int ScorePumpkinsDiagonal()
    {
        int result = 0;
        int[] scoreArray = { 0, 4, 9, 15, 22, 30, 39 };
        int pumpkinCounter = 0;

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                if (gridList[i][j] == 2)
                {
                    bool isOnlyOne = true;
                    //Left Up
                    for (int k = 1; i-k >=0 && j-k>=0; k++)
                    {
                        if (gridList[i - k][j - k] == 2) isOnlyOne = false;
                    }

                    //Right Up
                    for (int k = 1; i - k >= 0 && j + k < gridList[i-k].Count; k++)
                    {
                        if (gridList[i - k][j + k] == 2) isOnlyOne = false;
                    }

                    //Left Down
                    for (int k = 1; i + k < gridList.Count && j - k >= 0; k++)
                    {
                        if (gridList[i + k][j - k] == 2) isOnlyOne = false;
                    }

                    //Right Down
                    for (int k = 1; i + k < gridList.Count && j + k < gridList[i+k].Count; k++)
                    {
                        if (gridList[i + k][j + k] == 2) isOnlyOne = false;
                    }

                    if (isOnlyOne) pumpkinCounter++;
                }
            }
        }

        result = scoreArray[pumpkinCounter];
        return result;
    }


    void GetRandomGrid()
    {
        int size = Random.Range(4, 6);
        gridList.Clear();

        for (int i = 0; i< size; ++i)
        {
            List<int> tempList = new List<int>();
            for (int j = 0; j<size; ++j)
            {
                tempList.Add(Random.Range(2,6));
                if (Random.value < treesRatio) tempList[j] = 1;
                
            }
            gridList.Add(tempList);
        }
        gridList[Random.Range(0, size)][Random.Range(0, size)] = 6;
    }

    void PrintGrid()
    {
        string toPrint = "Grid List:\n";
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                toPrint += gridList[i][j] + ", ";

            }
            toPrint += "\n";
           
        }
        print(toPrint);
    }

    

    //Promień krzyżowy
    //for each heart wisp
    //1p for every tree in the same column
    //1p for every tree in the same row
    int ScoreHeartsCross()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 3)
                {
                    for (int k = 0; k < gridList.Count; ++k)
                    {
                        if (gridList[i][k] == 1) result++;
                        if (gridList[k][j] == 1) result++;
                    }
                }

            }

        }
        return result;
    }

    int ScoreHeartsAround()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 3)
                {
                    if (i - 1 >= 0 && gridList[i - 1][j] == 1) result += 2;
                    if (j - 1 >= 0 && gridList[i][j - 1] == 1) result += 2;
                    if (i + 1 < gridList.Count && gridList[i + 1][j] == 1) result += 2;
                    if (j + 1 < gridList[i].Count && gridList[i][j + 1] == 1) result += 2;
                   
                }

            }
        }
        return result;
    }

    //w obecnym ustawieniu dla rzędów mniejszych
    int ScoreHeartsColumnBelow()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 3)
                {
                    for (int k = 0; k < j; ++k)
                    {
                        if (gridList[i][k] == 1) result += 2;
                    }

                }

            }
        }
        return result;
    }

    //2punkty za każde drzewo na jednej z dwóch ukośnych linii
    int ScoreHeartsRays()
    {
        int result = 0;

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 3)
                {
                    int diagonalRisingCounter = 0;
                    int diagonalFallingCounter = 0;

                    //Left Up
                    for (int k = 1; i - k >= 0 && j - k >= 0; k++)
                    {
                        if (gridList[i - k][j - k] == 1) diagonalFallingCounter++;
                    }

                    //Right Up
                    for (int k = 1; i - k >= 0 && j + k < gridList[i - k].Count; k++)
                    {
                        if (gridList[i - k][j + k] == 1) diagonalRisingCounter++;
                    }

                    //Left Down
                    for (int k = 1; i + k < gridList.Count && j - k >= 0; k++)
                    {
                        if (gridList[i + k][j - k] == 1) diagonalRisingCounter++;
                    }

                    //Right Down
                    for (int k = 1; i + k < gridList.Count && j + k < gridList[i + k].Count; k++)
                    {
                        if (gridList[i + k][j + k] == 1) diagonalFallingCounter++;
                    }

                    result += 2 * Mathf.Max(diagonalFallingCounter, diagonalRisingCounter);
                }
            }
        }

        return result;

    }

    int ScoreHeartsLongestLine()
    {
        int result = 0;

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 3)
                {
                    int upCounter = 0;
                    for (int k = 1; i - k > 0; ++k)
                    {
                        if (gridList[i-k][j] == 1) upCounter++;
                        else break;
                    }

                    int downCounter = 0;
                    for (int k = 1; i + k < gridList.Count; ++k)
                    {
                        if (gridList[i+k][j] == 1) downCounter++;
                        else break;
                    }

                    int leftCounter = 0;
                    for (int k = 1; j - k > 0; ++k)
                    {
                        if (gridList[i][j-k] == 1) leftCounter++;
                        else break;
                    }

                    int rightCounter = 0;
                    for (int k = 1; j + k < gridList[i].Count; ++k)
                    {
                        if (gridList[i][j + k] == 1) rightCounter++;
                        else break;
                    }

                    result += 2 * Mathf.Max(upCounter, downCounter, leftCounter, rightCounter);
                }
            }
        }

        return result;
    }

    int ScoreWitchesUniversal()
    {
        int subScore = 0;
        int counter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                if (gridList[i][j] == 4) counter++ ;

            }

        }
        print("c" + counter);
        subScore = 18 * (counter / 3);
        if ((counter % 3) == 2) subScore += 10;
        else if ((counter % 3) == 1) subScore += 4;
        return subScore;
    }


    int ScoreOrbsAround()
    {
        int result = 0;

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 5)
                {
                    List<int> tempList = new List<int>();
                    for (int k = (i>0) ? -1 : 0; k < Mathf.Min(2,gridList.Count - i); ++k)
                    {
                        for (int l = (j>0) ? -1 : 0; l < Mathf.Min(2, gridList[i].Count - j); ++l)
                        {
                            if (k == 0 && l == 0) continue;
                            //print("orb: " + (i+k)  + " , " + (j+l) + "= " + gridList[i + k][j + l]);
                            if (gridList[i + k][j + l] > 1 && gridList[i + k][j + l] < 6 && !tempList.Contains(gridList[i + k][j + l])) tempList.Add(gridList[i + k][j+l]);
                        }

                    }
                    result += tempList.Count * 2;
                }

            }

        }
        return result;
    }


    int ScoreOrbsTunnel()
    {
        int result = 0;

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 5)
                {
                    List<int> tempList1 = new List<int>();
                    List<int> tempList2 = new List<int>();
                    for (int k = 0; k < gridList.Count; ++k)
                    { 
                        if (gridList[k][j] > 1 && gridList[k][j] < 6 && !tempList1.Contains(gridList[k][j])) tempList1.Add(gridList[k][j]);
                        if (gridList[i][k] > 1 && gridList[i][k] < 6 && !tempList2.Contains(gridList[i][k])) tempList2.Add(gridList[i][k]);
                    }
                    result += Mathf.Max(tempList1.Count, tempList2.Count) * 2;
                }

            }

        }
        return result;
    }

    int ScoreOrbsNeedle()
    {
        List<int> typeList = new List<int> {0,0,0,0,0,0,0};
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                typeList[gridList[i][j]]++;

            }

        }

        int minIndex = 0;
        int minAmount = 99;
        for (int i =2; i<6; ++i)
        {
            if (typeList[i] < minAmount)
            {
                minIndex = i;
                minAmount = typeList[i];
            }
        }
        int[] pointArray = new[] { 0, 0, 5, 3, 4, 7 };
        int result = pointArray[minIndex] * typeList[5];
        return result;
    }

    int ScoreOrbsSynergy()
    {
        int result = 0;
        int[] pointsArray = new int[] { 99, 99, 5, 7, 6, 4, 99 };
        for (int i = 0; i < gridList.Count; ++i)
        {
            for(int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 5)
                {
                    int orbPoints = 99;
                    if (i - 1 >= 0 && pointsArray[gridList[i - 1][j]] < orbPoints) orbPoints = pointsArray[gridList[i - 1][j]];
                    if (j - 1 >= 0 && pointsArray[gridList[i][j - 1]] < orbPoints) orbPoints = pointsArray[gridList[i][j - 1]];
                    if (i + 1 < gridList.Count && pointsArray[gridList[i + 1][j]] < orbPoints) orbPoints = pointsArray[gridList[i + 1][j]];
                    if (j + 1 < gridList[i].Count && pointsArray[gridList[i][j + 1]] < orbPoints) orbPoints = pointsArray[gridList[i][j + 1]];
                    print("orbs: " + orbPoints);
                    if (orbPoints == 99) orbPoints = 0;
                    result += orbPoints;
                }
            }
        }
        return result;
    }

   

    int ScoreOrbsGroups()
    {
        int result = 0;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        List<Vector2Int> visited = new List<Vector2Int>();
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                if (gridList[i][j] == 5 && !visited.Contains(new Vector2Int(i, j)))
                {
                    queue.Enqueue(new Vector2Int(i, j));
                    List<int> wispTypesInGroup = new List<int>();
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;


                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }


                        if (!wispTypesInGroup.Contains(gridList[x][y])) wispTypesInGroup.Add(gridList[x][y]);

                        if (x > 0 && IsWisp(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x - 1, y));
                        if (y > 0 && IsWisp(x , y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y - 1));
                        if (x < gridList.Count - 1 && IsWisp(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count - 1 && IsWisp(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                    result += wispTypesInGroup.Count * 2;
                }
            }
        }

        return result;
    }

    int ScoreOrbsGroupsCorners()
    {
        int result = 0;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        List<Vector2Int> visited = new List<Vector2Int>();
        Dictionary <Vector2Int,int> wispGroup = new Dictionary<Vector2Int,int>();
        List<int> groupPoints = new List<int>();
        int groupCounter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList[i].Count; ++j)
            {
                if (IsWisp(i,j) && !visited.Contains(new Vector2Int(i, j)))
                {
                    queue.Enqueue(new Vector2Int(i, j));
                    List<int> wispTypesInGroup = new List<int>();
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;


                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }

                        wispGroup[new Vector2Int(x, y)] = groupCounter;

                        if (!wispTypesInGroup.Contains(gridList[x][y])) wispTypesInGroup.Add(gridList[x][y]);

                        if (x > 0 && IsWisp(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x - 1, y));
                        if (y > 0 && IsWisp(x, y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y - 1));
                        if (x < gridList.Count - 1 && IsWisp(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count - 1 && IsWisp(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                    groupPoints.Add(wispTypesInGroup.Count * 2);
                    groupCounter++;
                }
            }
        }

        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 5)
                {
                    int orbScore = 0;

                    Vector2Int orbKey = new Vector2Int(i, j);

                    //Left Up
                    Vector2Int cornerKey = new Vector2Int(i - 1, j - 1); 
                    if (wispGroup.ContainsKey(cornerKey)
                        && wispGroup[orbKey] != wispGroup[cornerKey]) 
                        orbScore = Mathf.Max(orbScore, wispGroup[cornerKey]);

                    //Right Up
                    cornerKey = new Vector2Int(i - 1, j + 1);
                    if (wispGroup.ContainsKey(cornerKey)
                        && wispGroup[orbKey] != wispGroup[cornerKey])
                        orbScore = Mathf.Max(orbScore, wispGroup[cornerKey]);

                    //Left Down
                    cornerKey = new Vector2Int(i + 1, j - 1);
                    if (wispGroup.ContainsKey(cornerKey)
                        && wispGroup[orbKey] != wispGroup[cornerKey])
                        orbScore = Mathf.Max(orbScore, wispGroup[cornerKey]);

                    //Right Down
                    cornerKey = new Vector2Int(i + 1, j + 1);
                    if (wispGroup.ContainsKey(cornerKey)
                        && wispGroup[orbKey] != wispGroup[cornerKey])
                        orbScore = Mathf.Max(orbScore, wispGroup[cornerKey]);

                    result += orbScore;
                }
            }
        }

        return result;
    }

    bool IsWisp(int x, int y)
    {
        return gridList[x][y] >= 2 && gridList[x][y] <= 5;
    }

    int ScoreTreesRowColumn()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            int treeCounterRow = 0;
            int treeCounterColumn = 0;
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 1) treeCounterRow++;
                if (gridList[j][i] == 1) treeCounterColumn++;

            }
            if (treeCounterRow >= 3) result += 2;
            if (treeCounterColumn >= 3) result += 2;

        }
        return result;
    }

    int ScoreTreesAround()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
             if (gridList[i][j] == 1)
                {
                    int treeCounter = 0;
                    if (i - 1 >= 0 && gridList[i - 1][j] == 1) treeCounter++;
                    if (j - 1 >= 0 && gridList[i][j - 1] == 1) treeCounter++;
                    if (i + 1 < gridList.Count && gridList[i + 1][j] == 1) treeCounter++;
                    if (j + 1 < gridList[i].Count && gridList[i][j + 1] == 1) treeCounter++;
                    if (treeCounter >= 3) result += 3;
                }
                
            }
        }
        return result;
    }

    //1 punkt za każde drzewo w największej grupie drzew
    int ScoreTreesBiggestGroup()
    {
        int result = 0;
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int biggestSize = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 1 && !visited.Contains(new Vector2Int(i, j)))
                {
                    int groupSize = 0;
                    queue.Enqueue(new Vector2Int(i, j));
                    List<int> wispTypesInGroup = new List<int>();
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;


                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }

                        groupSize++;

                        if (x > 0 && IsTree(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x - 1, y));
                        if (y > 0 && IsTree(x, y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y - 1));
                        if (x < gridList.Count - 1 && IsTree(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count - 1 && IsTree(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                    if (groupSize > biggestSize) biggestSize = groupSize;
                    
                }

            }
        }
        result = biggestSize;
        return result;
    }

    //2 punkty za każde drzewo w drugiej największej grupie drzew
    int ScoreTreesSecondBiggestGroup()
    {
        int result = 0;
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int biggestSize = 0;
        int secondBiggestSize = 0; //xD
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 1 && !visited.Contains(new Vector2Int(i, j)))
                {
                    int groupSize = 0;
                    queue.Enqueue(new Vector2Int(i, j));
                    List<int> wispTypesInGroup = new List<int>();
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;


                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }

                        groupSize++;

                        if (x > 0 && IsTree(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x - 1, y));
                        if (y > 0 && IsTree(x, y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y - 1));
                        if (x < gridList.Count - 1 && IsTree(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count - 1 && IsTree(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                    if (groupSize > biggestSize)
                    {
                        secondBiggestSize = biggestSize;
                        biggestSize = groupSize;
                    }
                    else if (groupSize > secondBiggestSize)
                        secondBiggestSize = groupSize;

                }

            }
        }
        result = 2 * secondBiggestSize;
        return result;
    }

    /// <summary>
    /// 4 punkty za każdą grupę składającą się z conajmniej trzech Drzew
    /// </summary>
    int ScoreTreesGroupsBiggerThanThree()
    {
        int result = 0;
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int groupsCounter = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 1 && !visited.Contains(new Vector2Int(i, j)))
                {
                    int groupSize = 0;
                    queue.Enqueue(new Vector2Int(i, j));
                    List<int> wispTypesInGroup = new List<int>();
                    while (queue.Count > 0)
                    {
                        int x = queue.Peek().x;
                        int y = queue.Peek().y;


                        if (visited.Contains(new Vector2Int(x, y)))
                        {
                            queue.Dequeue();
                            continue;
                        }

                        groupSize++;

                        if (x > 0 && IsTree(x - 1, y) && !queue.Contains(new Vector2Int(x - 1, y))) queue.Enqueue(new Vector2Int(x - 1, y));
                        if (y > 0 && IsTree(x, y - 1) && !queue.Contains(new Vector2Int(x, y - 1))) queue.Enqueue(new Vector2Int(x, y - 1));
                        if (x < gridList.Count - 1 && IsTree(x + 1, y) && !queue.Contains(new Vector2Int(x + 1, y))) queue.Enqueue(new Vector2Int(x + 1, y));
                        if (y < gridList[x].Count - 1 && IsTree(x, y + 1) && !queue.Contains(new Vector2Int(x, y + 1))) queue.Enqueue(new Vector2Int(x, y + 1));

                        visited.Add(queue.Dequeue());
                    }
                    if (groupSize >= 3) groupsCounter++;

                }

            }
        }
        result = 4 * groupsCounter;
        return result;
    }

    bool IsTree(int x, int y)
    {
        return gridList[x][y] == 1;
    }

    int ScoreFullForest()
    {
        
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] == 0) return 0;

            }

        }
        return 2 * (gridList.Count - 3);
    }





}
