using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public string[] headers;
    public string[] contents;
    public GameObject[] highlighted;

    public TextMeshProUGUI headerLabel;
    public TextMeshProUGUI contentLabel;
    public GameObject tutorialPanel;
    public TextMeshProUGUI buttonText;

    [Header("Highlighten")]
    public Color baseColor = Color.white;
    public Color blinkColor = Color.aliceBlue;
    public float timeInterval = 0.6f;

    public int tutIndex = 0;
    public int jumpedFrom = -1;

    float counter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialPanel.SetActive(false);
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
            counter += Time.deltaTime;
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
        if (tutIndex < headers.Length) SetLabels();
        else End();
    }

    public void JumpTo(int i)
    {
        jumpedFrom = tutIndex;
        tutIndex = 0;
        SetLabels();
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    void SetLabels()
    {
        tutorialPanel.SetActive(headers[tutIndex] != "");
        headerLabel.text = headers[tutIndex];
        contentLabel.text = contents[tutIndex];
        if (highlighted[tutIndex-1] != null) highlighted[tutIndex-1].SetActive(false);
        if (highlighted[tutIndex] != null) highlighted[tutIndex].SetActive(true);
        buttonText.text = tutIndex == headers.Length - 1 ? "Zamknij" : (headers[tutIndex + 1] == "" ? "OK" : "Dalej");

    }

}
