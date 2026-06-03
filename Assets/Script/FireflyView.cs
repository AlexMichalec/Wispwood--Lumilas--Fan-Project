using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyView : MonoBehaviour
{

    public GameObject fireflyPrefab;
    public Material[] materials;
    public Transform topLeft;
    public Transform bottomRight;
    public Transform preparedPoint;
    public Transform revealedPoint;
    public float spawnHeight = 0.1f;
    public float flipHeight = 1;
    public float flipTime = 0.5f;
    public float minDistance = 0.1f;
    public float breakBeforeFlipping = 0.5f;
    public float delayBeforeEnemyMove = 2;
    public float delayBeforeNextFirefly = 1;
    public int mixFlipsAmount = 24;
    [Header("Navigation")]
    public UI userInterface;
    public MoveCamera cameraMover;
    public EnemyManager enemyManager;
    public GameManager gameManager;
    public Tutorial tutorial;

    private int[] values = { 1, 2, 2, 2, 3, 3, 3, 4 };
    private int round = 1;
    private List<GameObject> allFireflies = new List<GameObject>();
    private List<GameObject> chosenFireflies = new List<GameObject>();
    private List<GameObject> shownFireflies = new List<GameObject>();
    private List<Vector3> goalPositions = new List<Vector3>();
    private List<GameObject> inAir = new List<GameObject>();
    private bool firstFireflyInGame = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator InitFireflies()
    {
        StartCoroutine(cameraMover.PeekFireflies());
        yield return new WaitForSeconds(cameraMover.fullTime / 2);
        foreach (GameObject firefly in allFireflies) Destroy(firefly);
        allFireflies.Clear();

        foreach (int x in values)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject newFirefly = Instantiate(fireflyPrefab, transform);
            newFirefly.transform.GetChild(0).GetComponent<Renderer>().material = materials[x];
            newFirefly.transform.position = GetNewPosition();
            newFirefly.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allFireflies.Add(newFirefly);
        }
        yield return new WaitForSeconds(breakBeforeFlipping);
        StartCoroutine(FlipAll());
    }

    IEnumerator FlipAll()
    {
        StartCoroutine(userInterface.EnemyActionUpdate("Losuję świetliki dla Ducha"));
        if (gameManager.tutorialMode) tutorial.Next();
        foreach (GameObject firefly in allFireflies)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(FlipFirefly(firefly));
        }

        yield return new WaitForSeconds(flipTime);
        StartCoroutine(MixFireflies());

    }

    IEnumerator FlipFirefly(GameObject firefly)
    {
        float time = flipTime;
        float counter = 0;
        Vector3 startPos = firefly.transform.position;
        Vector3 endPos = startPos + new Vector3(0, flipHeight, 0);
        Quaternion startRot = firefly.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(180, 0, 0);

        while (counter <= time)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / time);
            if (counter < time/2) firefly.transform.position = Vector3.Lerp(startPos, endPos, 2 * t);
            else firefly.transform.position = Vector3.Lerp(endPos, startPos, 2 * (t - 0.5f));
            firefly.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

        }
    }

    Vector3 GetNewPosition()
    {
        bool isOkay = false;
        int counter = 0;
        Vector3 newPosition = new Vector3();
        while (!isOkay && counter < 100)
        {
            counter++;
            isOkay = true;
            newPosition = new Vector3(UnityEngine.Random.Range(topLeft.position.x, bottomRight.position.x), spawnHeight, UnityEngine.Random.Range(topLeft.position.z, bottomRight.position.z));
            foreach (GameObject firefly in allFireflies)
            {
                if ((firefly.transform.position - newPosition).magnitude < minDistance) isOkay = false;
            }

            foreach (Vector3 firePos in goalPositions)
            {
                if ((firePos - newPosition).magnitude < minDistance) isOkay = false;
            }
        }
        if (counter >= 100) print("MinDistance too big!");
        return newPosition;
    }

    public IEnumerator ChooseFirefliesForRound()
    {
        
        chosenFireflies.Clear();
        //yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < round*2 + 2; ++i)
        {
            int index = UnityEngine.Random.Range(0, allFireflies.Count);
            while (chosenFireflies.Contains(allFireflies[index])) index = UnityEngine.Random.Range(0, allFireflies.Count);
            GameObject firefly = allFireflies[index];
            chosenFireflies.Add(firefly);
            //allFireflies[index].transform.rotation = Quaternion.Euler(180, 180, 0);
            //allFireflies[index].transform.position = preparedPoint.position + new Vector3(0,(i+1)*0.05f,0);
            //print(GetValue(chosenFireflies[chosenFireflies.Count - 1]));

            float time = flipTime;
            float counter = 0;
            Vector3 startPos = allFireflies[index].transform.position;
            Vector3 endPos = preparedPoint.position + new Vector3(0, (i + 1) * 0.05f, 0);
            Quaternion startRot = allFireflies[index].transform.rotation;
            Quaternion endRot = Quaternion.Euler(180, 180, 0);
            
            while (counter <= time)
            {
                yield return null;
                counter += Time.deltaTime;
                float t = Mathf.Clamp01(counter / time);
                firefly.transform.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, flipHeight * Mathf.Sin(t * Mathf.PI), 0);
                firefly.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            }
            //yield return new WaitForSeconds(0.5f);
            

        }

        userInterface.InitializeFireflies(round * 2 + 2);
        yield return new WaitForSeconds(delayBeforeNextFirefly);

        StartCoroutine(NextFirefly());
    }

    public IEnumerator NextFirefly(bool isFirstInRound = false)
    {
        if (!isFirstInRound)
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(cameraMover.PeekFirefliesCloser());
            yield return new WaitForSeconds(cameraMover.fullTime);

        }
        if (gameManager.tutorialMode && firstFireflyInGame) 
        {
            //tutorial.Next();
        }
        float time = flipTime;
        float counter = 0;
        GameObject firefly = chosenFireflies[chosenFireflies.Count - 1];
        chosenFireflies.Remove(firefly);
        Vector3 startPos = firefly.transform.position;
        Vector3 endPos = revealedPoint.position + new Vector3(0, 0.05f * (round*2 +3 - chosenFireflies.Count), 0);
        Quaternion startRot = firefly.transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, 180, 0);
        while(counter <= time)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter/time);
            firefly.transform.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0,flipHeight * Mathf.Sin(t * Mathf.PI),0);
            firefly.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        }
        shownFireflies.Add(firefly);
        userInterface.UpdateFireflies(GetValue(firefly), chosenFireflies.Count);
        if (gameManager.tutorialMode && firstFireflyInGame)
        {
            tutorial.NextDelay(0.2f); //???
            firstFireflyInGame = false;
        }

        yield return new WaitForSeconds(delayBeforeEnemyMove);
        StartCoroutine(cameraMover.moveCamera());
        StartCoroutine(enemyManager.CollectWisp(cameraMover.fullTime, GetValue(firefly), chosenFireflies.Count == 0));
    }

    int GetValue(GameObject firefly)
    {
        return values[allFireflies.IndexOf(firefly)];
    }

    public IEnumerator NextRound()
    {
        StartCoroutine(cameraMover.PeekFireflies());
        yield return new WaitForSeconds(cameraMover.fullTime / 2);
        List<GameObject> firefliesToFlipBack = new List<GameObject>();
        chosenFireflies.Reverse();
        shownFireflies.Reverse();
        firefliesToFlipBack.AddRange(chosenFireflies);
        firefliesToFlipBack.AddRange(shownFireflies);
        foreach(GameObject firefly in firefliesToFlipBack)
        {

            yield return new WaitForSeconds(0.1f);
            float time = flipTime;
            float counter = 0;
            Vector3 startPos = firefly.transform.position;
            Vector3 endPos = GetNewPosition();
            Quaternion startRot = firefly.transform.rotation;
            Quaternion endRot = Quaternion.Euler(180,UnityEngine.Random.Range(0,360),0);

            while (counter <= time)
            {
                yield return null;
                counter += Time.deltaTime;
                float t = Mathf.Clamp01(counter / time);
                firefly.transform.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, flipHeight * Mathf.Sin(t * Mathf.PI), 0);
                firefly.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            }
        }
        chosenFireflies.Clear();
        shownFireflies.Clear();
        round++;
        StartCoroutine(userInterface.EnemyActionUpdate("Losuję świetliki na kolejną rundę"));
        StartCoroutine(MixFireflies());
    } 

    IEnumerator MixFireflies()
    {
        
        for (int i =0; i < mixFlipsAmount; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject firefly = allFireflies[UnityEngine.Random.Range(0, allFireflies.Count)];
            while (inAir.Contains(firefly)) firefly = allFireflies[UnityEngine.Random.Range(0, allFireflies.Count)];
            inAir.Add(firefly);
            StartCoroutine(FlipToMix(firefly));


        }
        yield return new WaitForSeconds(2*flipTime/3);
        StartCoroutine(ChooseFirefliesForRound());
    }

    IEnumerator FlipToMix(GameObject firefly)
    {
        float time = flipTime/2;
        float counter = 0;
        Vector3 startPos = firefly.transform.position;
        Vector3 endPos = GetNewPosition();
        goalPositions.Add(endPos);
        Quaternion startRot = firefly.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        while (counter <= time)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / time);
            firefly.transform.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, flipHeight/2 * Mathf.Sin(t * Mathf.PI), 0);
            firefly.transform.rotation = Quaternion.Lerp(startRot, endRot, t);

        }
        goalPositions.Remove(endPos);
        inAir.Remove(firefly);
    }

}
