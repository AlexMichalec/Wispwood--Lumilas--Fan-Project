using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{
    public string[] headers;
    public string[] contents;
    public GameObject[] highlighted;
    public GameObject[] toShow;

    public TextMeshProUGUI headerLabel;
    public TextMeshProUGUI contentLabel;
    public GameObject tutorialPanel;
    public TextMeshProUGUI buttonText;
    public GameObject tutorialBackground;

    [Header("Highlighten")]
    public Color baseColor = Color.white;
    public Color blinkColor = Color.aliceBlue;
    public float timeInterval = 0.6f;

    public int tutIndex = 0;
    public int jumpedFrom = -1;

    public int treeTurnInfoIndex = 36;
    public bool treeTurnExplained = false;

    float counter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialPanel.SetActive(false);
        tutorialBackground.SetActive(false);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Next();
        }

        if (Input.GetKeyDown(KeyCode.P)) Time.timeScale = Time.timeScale == 1 ? 0 : 1;

        if (highlighted[tutIndex] != null)
        {
            counter += Time.unscaledDeltaTime;
            if (counter > timeInterval)
            {
                Blink();
                counter = 0;
            }
        }
    }

    void Blink()
    {
        for (int i = 0; i < highlighted[tutIndex].transform.childCount; ++i)
        {
            print("b?");
            GameObject child = highlighted[tutIndex].transform.GetChild(i).gameObject;
            if (child.GetComponent<Image>())
            {
                child.GetComponent<Image>().color = child.GetComponent<Image>().color == baseColor ? blinkColor : baseColor;
            }

        }
    }

    public void Next()
    {
        tutIndex++;
        if (tutIndex == 1)
        {
            foreach (GameObject sth in toShow)
            {
                if (sth == null) continue;
                sth.SetActive(false);
            }
        }
        if (tutIndex == treeTurnInfoIndex && treeTurnExplained)
        {
            while (headers[tutIndex] != "") ++tutIndex;
            ++tutIndex;
        }
        if (tutIndex < headers.Length) SetLabels();
        else End();
    }

    IEnumerator NextDelaying(float delay)
    {
        yield return new WaitForSeconds(delay);
        Next();
    }

    public void NextDelay(float delay)
    {
        StartCoroutine(NextDelaying(delay));
    }

    public void JumpTo(int i)
    {
        jumpedFrom = tutIndex;
        tutIndex = i;
        SetLabels();
    }
    IEnumerator JumpDelaying(int i, float delay)
    {
        yield return new WaitForSeconds(delay);
        JumpTo(i);
    }

    public void JumpToDelay(int i, float delay)
    {
        StartCoroutine(JumpDelaying(i, delay));
    }


    public void End()
    {
        gameObject.SetActive(false);
    }

    void SetLabels()
    {
        if (tutIndex == treeTurnInfoIndex) treeTurnExplained = true;
        tutorialPanel.SetActive(headers[tutIndex] != "");
        tutorialBackground.SetActive(headers[tutIndex] != "");
        Time.timeScale = headers[tutIndex] == "" ? 1 : 0;
        headerLabel.text = headers[tutIndex];
        contentLabel.text = contents[tutIndex];
        if (highlighted[tutIndex-1] != null) highlighted[tutIndex-1].SetActive(false);
        if (highlighted[tutIndex] != null) highlighted[tutIndex].SetActive(true);
        if (toShow[tutIndex] != null) toShow[tutIndex].SetActive(true);
        buttonText.text = tutIndex == headers.Length - 1 ? "Zamknij" : (headers[tutIndex + 1] == "" ? "OK" : "Dalej");
        if (headers[tutIndex] == "" && jumpedFrom != -1)
        {
            tutIndex = jumpedFrom;
            jumpedFrom = -1;
        }
    }

}
