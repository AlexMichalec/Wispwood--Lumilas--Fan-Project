using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using UnityEngine.Rendering.LookDev;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyTilePrefab;
    private GameObject enemyTile;

    private int[] fireflies = { 1, 1, 2, 2, 3, 3, 4, 4 };
    private Queue<int> currentFireflies;

    public int difficultyLevel = 0;
    private List<int> wispMultipliers;
    private List<int> collectedWisps;
    private Dictionary<int, int> wispDict;

    int score = 0;
    private int enemyPondIndex = 0;

    public float jumpHeight = 1.5f;
    public float jumpTime = 0.5f;

    private List<GameObject> pondTiles;
    public GameManager gameManager;
    public UI userInterface;

    private List<int> scoreArray = new List<int>();

    public void ChooseFireflies()
    {
        int amount = 2 + 2* gameManager.round;
        for (int i = 0; i <40; ++i) //Shuffle Fireflies
        {
            int a = Random.Range(0, fireflies.Length);
            int b = Random.Range(0, fireflies.Length);
            (fireflies[a], fireflies[b]) = (fireflies[b], fireflies[a]);
        }
        currentFireflies = new Queue<int>();
        for (int i = 0; i < amount; ++i) currentFireflies.Enqueue(fireflies[i]);
        userInterface.InitializeFireflies(amount);
        

    }

    public void SetWispsMultipliers()
    {
        ResetCollectedWisps();
        List<List<int>> wispsMultipliersTemplate = new List<List<int>> { new List<int> {6, 5, 4, 3}, new List<int> {7, 6, 5 ,4}, new List<int> { 8, 6, 5, 4 }, new List<int> { 9, 7, 6, 4 } };
        wispMultipliers = wispsMultipliersTemplate[difficultyLevel];
        for (int _ = 0; _ < 20; ++_)
        {
            int a = Random.Range(0, wispMultipliers.Count);
            int b = Random.Range(0, wispMultipliers.Count);
            (wispMultipliers[a], wispMultipliers[b]) = (wispMultipliers[b], wispMultipliers[a]);
        }
        wispDict = new Dictionary<int, int>();
        wispsMultipliersTemplate = new List<List<int>> { new List<int> { 6, 5, 4, 3 }, new List<int> { 7, 6, 5, 4 }, new List<int> { 8, 6, 5, 4 }, new List<int> { 9, 7, 6, 4 } };

        for (int i =0; i < wispMultipliers.Count; ++i)
        {
            wispDict[i] = wispsMultipliersTemplate[difficultyLevel].IndexOf(wispMultipliers[i]);
        }
        StartCoroutine(userInterface.InitializeGhostScoreboard(wispMultipliers));
    }

    void ResetCollectedWisps()
    {
        collectedWisps = new List<int> { 0, 0, 0, 0 };
    }

    public void SpawnEnemy(Vector3 pos, int index)
    {
        enemyTile = Instantiate(enemyTilePrefab, pos, enemyTilePrefab.transform.rotation);
        pondTiles = gameManager.GetPondTiles();
        pondTiles[index] = enemyTile;
        enemyPondIndex = index;
    }

    public IEnumerator CollectWisp(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        int howFar = currentFireflies.Dequeue();
        userInterface.UpdateFireflies(howFar, currentFireflies.Count);


        yield return new WaitForSeconds(1.5f);

        List<GameObject> choosePool = new List<GameObject>();
        int i = enemyPondIndex;
        while (choosePool.Count < howFar)
        {
            i = (i + 1) % pondTiles.Count;
            if (i == enemyPondIndex)
            {
                choosePool = new List<GameObject>();
                gameManager.DealNewWisps();
                yield return new WaitForSeconds(2);
                StartCoroutine(userInterface.EnemyActionUpdate("Za mało duszków! Duch wezwał nową porcję...", 3));
                yield return new WaitForSeconds(2);
            }
            else if (pondTiles[i] != null)
            {
                choosePool.Add(pondTiles[i]);
            }

        }
        GameObject chosenTile = choosePool[0];
        gameManager.LightUp(choosePool);
        yield return new WaitForSeconds(2);

        for (int j =1; j< choosePool.Count; ++j)
        {
            int valueChosen = wispMultipliers[chosenTile.GetComponent<TileScript>().wispType];
            int newValue = wispMultipliers[choosePool[j].GetComponent<TileScript>().wispType];
            if (newValue > valueChosen) chosenTile = choosePool[j];
        }


        int chosenWisp = chosenTile.GetComponent<TileScript>().wispType;
        collectedWisps[chosenWisp]++;

        float tCounter = 0;
        Vector3 oldPos = enemyTile.transform.position;
        Vector3 newPos = chosenTile.transform.position;

        while (tCounter < jumpTime)
        {
            enemyTile.transform.position = Vector3.Lerp(oldPos, newPos, tCounter / jumpTime) + Vector3.up* jumpHeight * Mathf.Sin(Mathf.PI * (tCounter/jumpTime));
            tCounter += Time.deltaTime;
            yield return null;
        }
        enemyTile.transform.rotation = Quaternion.Euler(0, 0, 0);

        gameManager.DarkOut(choosePool);
        int newIndex = pondTiles.IndexOf(chosenTile);
        Destroy(chosenTile);

        pondTiles[enemyPondIndex] = null;
        enemyPondIndex = newIndex;
        pondTiles[enemyPondIndex] = enemyTile;
        enemyTile.transform.position = gameManager.GetPlatformPosition(enemyPondIndex) + new Vector3(0,0.1f,0);

        
        gameManager.NextPlayer(currentFireflies.Count == 0);

        yield return new WaitForSeconds(1);
        userInterface.UpdateGhostScore(collectedWisps, chosenWisp);
    }

    int GetSimpleScore()
    {
        for (int i = 0; i < collectedWisps.Count; ++i)
        {
            score += collectedWisps[i] * wispMultipliers[i];
        }
        return score;
    }

    public int GetFinalScore()
    {
        if (scoreArray.Count < 16) return GetSimpleScore();
        return scoreArray[15];
    }

    public void ScoreRound()
    {
        
        if (scoreArray.Count == 0)
        {
            for (int i = 0; i < 16; ++i)
            {
                scoreArray.Add(0);
            }
        }
        int oldTotalSum = scoreArray[15];
        int round = gameManager.round -1;
        for (int i = 0; i < collectedWisps.Count; ++i)
        {
            int scoreIndex = wispDict[i];
            scoreArray[scoreIndex*3+round] = collectedWisps[i] * wispMultipliers[i];
            scoreArray[12+round] += collectedWisps[i] * wispMultipliers[i];
            scoreArray[15] += collectedWisps[i] * wispMultipliers[i];
        }
        string g = "";
        for (int i = 0; i < scoreArray.Count; ++i) g += scoreArray[i] + ", ";
        Debug.Log(g);
        StartCoroutine(userInterface.ShowEnemyScoreRound(scoreArray, oldTotalSum));
    }

    void Start()
    {

    }

 
}
