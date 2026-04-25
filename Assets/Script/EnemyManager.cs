using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyTilePrefab;
    private GameObject enemyTile;

    private int[] fireflies = { 1, 1, 2, 2, 3, 3, 4, 4 };
    private Queue<int> currentFireflies;

    public int difficultyLevel = 0;
    private List<int> wispMultipliers;
    private List<int> collectedWisps;

    int score = 0;
    private int enemyPondIndex = 0;

    private List<GameObject> pondTiles;
    public GameManager gameManager;
    public UI userInterface;

    void ChooseFireflies()
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

    void SetWispsMultipliers()
    {
        List<List<int>> wispsMultipliersTemplate = new List<List<int>> { new List<int> {6, 5, 4, 3}, new List<int> {7, 6, 5 ,4}, new List<int> { 8, 6, 5, 4 }, new List<int> { 9, 7, 6, 4 } };
        wispMultipliers = wispsMultipliersTemplate[difficultyLevel];
        for (int _ = 0; _ < 20; ++_)
        {
            int a = Random.Range(0, wispMultipliers.Count);
            int b = Random.Range(0, wispMultipliers.Count);
            (wispMultipliers[a], wispMultipliers[b]) = (wispMultipliers[b], wispMultipliers[a]);
        }
        userInterface.InitializeGhostScoreboard(wispMultipliers);
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

    IEnumerator CollectWisp()
    {
        yield return null;
        int howFar = currentFireflies.Dequeue();
        userInterface.UpdateFireflies(howFar, currentFireflies.Count);

        List<GameObject> choosePool = new List<GameObject>();
        int i = enemyPondIndex;
        while (choosePool.Count < howFar)
        {
            i = (i + 1) % pondTiles.Count;
            if (i == enemyPondIndex)
            {
                choosePool = new List<GameObject>();
                gameManager.DealNewWisps();
                yield return new WaitForSeconds(1);
            }
            else if (pondTiles[i] != null)
            {
                choosePool.Add(pondTiles[i]);
            }

        }
        GameObject chosenTile = choosePool[0];
        for (int j =1; j< choosePool.Count; ++j)
        {
            int valueChosen = wispMultipliers[chosenTile.GetComponent<TileScript>().wispType];
            int newValue = wispMultipliers[choosePool[j].GetComponent<TileScript>().wispType];
            if (newValue > valueChosen) chosenTile = choosePool[j];
        }

        int chosenWisp = chosenTile.GetComponent<TileScript>().wispType;
        collectedWisps[chosenWisp]++;

        Vector3 newPos = chosenTile.transform.position;
        int newIndex = pondTiles.IndexOf(chosenTile);
        Destroy(chosenTile);
        enemyTile.transform.position = newPos + new Vector3(0, 0.1f, 0);
        enemyPondIndex = newIndex;

        userInterface.UpdateGhostScore(collectedWisps);
        gameManager.NextPlayer(currentFireflies.Count == 0);
        if (currentFireflies.Count == 0)
        {
            print("GHOST SCORE: " + GetScore());
        }
    }

    public int GetScore()
    {
        for (int i = 0; i < collectedWisps.Count; ++i)
        {
            score += collectedWisps[i] * wispMultipliers[i];
        }
        return score;
    }

    void Start()
    {
        ChooseFireflies();
        SetWispsMultipliers();
        ResetCollectedWisps();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(CollectWisp());
    }
}
