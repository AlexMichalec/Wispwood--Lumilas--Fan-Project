using UnityEngine;
using System.Collections.Generic;

public class Score : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<List<int>> gridList = new List<List<int>>();
    [Range(0,1)]
    [Tooltip("Percent of grid hidden/covered in trees")]
    public float treesRatio = 0.5f;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0, 5)]
    public int pumpkinsScoreMethodIndex = 0;
    [Tooltip("0 means don't add it to the Score")]
    [Range(0,5)]
    public int heartScoreMethodIndex = 0;
    private UI userInterface;
    public int myScore = 0;
    void Start()
    {
        userInterface = GameObject.Find("UI").GetComponent<UI>();
        if (gridList.Count == 0) GetRandomGrid();
        PrintGrid();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetRandomGrid();
            PrintGrid();
        
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SumUpScore();

        }

    }

    void SumUpScore()
    {
        int result = 0;
        //ADD PUMPKIN SCORE
        result += ScorePumpkins();
        //ADD HEARTS SCORE
        result += ScoreHearts();
        //ADD WITCH SCORE
        result += ScoreWitchs();
        //ADD ORB SCORE
        result += ScoreWitchs();
        //If forest full add 2/4/6 points
        result += ScoreFullForest();
        myScore += result;
        userInterface.UpdateScore(myScore);
    }

    int ScorePumpkins()
    {
        int subScore = 0;
        if (pumpkinsScoreMethodIndex == 1) subScore = ScorePumpkinsSudoku();
        print("Pumpkins score: " + subScore);
        return subScore;
    }

    int ScoreHearts() 
    {
        int subScore = 0;
        if (heartScoreMethodIndex == 1) subScore = ScoreHeartsCross();
        print("Hearts score: " + subScore);
        return subScore;

    }
    int ScoreWitchs()
    {

        return 0;

    }

    int ScoreOrbs()
    {

        return 0;

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

    void CountNotTrees()
    {
        int result = 0;
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int j = 0; j < gridList.Count; ++j)
            {
                if (gridList[i][j] > 1) result++;

            }

        }
        userInterface.UpdateScore(result);
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
