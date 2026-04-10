using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;
using System.Collections.Generic;
using UnityEngine.InputSystem.Android;
public class UI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TMP_Dropdown[] scoreMethodDropdowns;
    public Color[] descriptionPanelColors;
    public Color basicPanelColor;
    public TextMeshProUGUI[] detailedScoreTexts;
    public TextMeshProUGUI[] descriptionTexts;
    public Image panelImage;
    public GameObject scoreNode;
    public TextMeshProUGUI topText;
    private Score scoreManager;
    public GameObject detailedScoreNode;
    public GameObject nextRoundButton;
    public int round = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Score: 0";
        scoreManager = scoreNode.GetComponent<Score>();
        InitializeScoreOptions();
        ShowScoreMethod(5);
        ResetDetailedScore();

    }

    void InitializeScoreOptions()
    {

        for (int i = 0; i < scoreMethodDropdowns.Length; ++i)
        {
            scoreMethodDropdowns[i].ClearOptions();
            for (int j = 0; j < 6; ++j)
            {
                scoreMethodDropdowns[i].AddOptions(new List<string> { Score.GetInfoScoreMethods(i + 1, j)[0] });
            }
            scoreMethodDropdowns[i].value = Random.Range(1, 6);
        }

        scoreMethodDropdowns[4].AddOptions(new List<string> { Score.GetInfoScoreMethods(5, 6)[0] });

        descriptionTexts[0].text = "Description Title";
        descriptionTexts[1].text = "Choose score method to see its description.";
        descriptionTexts[2].text = "Debug Only";
        panelImage.color = basicPanelColor;


    }


    public void UpdateScoreMethod(int index)
    {
        panelImage.color = descriptionPanelColors[index];
        string[] textsArray = Score.GetInfoScoreMethods(index + 1, scoreMethodDropdowns[index].value);
        descriptionTexts[2].text = "";
        for (int i = 0; i < textsArray.Length; ++i)
        {
            descriptionTexts[i].text = Score.GetInfoScoreMethods(index + 1, scoreMethodDropdowns[index].value)[i];
        }
        scoreManager.UpdateScoreMethod(index, scoreMethodDropdowns[index].value);
        //descriptionTexts[1].text = Score.GetInfoScoreMethods(index, scoreMethodDropdowns[index + 1].value)[0];
        //descriptionTexts[1].text = Score.GetInfoScoreMethods(index, scoreMethodDropdowns[index+1].value)[1];


    }

    public void ShowScoreMethod(int index)
    {
        panelImage.color = descriptionPanelColors[index];

        string[] textsArray = (index < 5) ? Score.GetInfoScoreMethods(index + 1, scoreMethodDropdowns[index].value) : new string[] { "Pełny Las", "Dodatkowe 2/4/6 punktów jeżeli wypełnisz cały swój las (4x4/5x5/6x6)" };
        descriptionTexts[2].text = "";

        for (int i = 0; i < textsArray.Length; ++i)
        {
            descriptionTexts[i].text = textsArray[i];
        }




    }


    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateDetailedScore(int[] scoreArray)
    {
        detailedScoreNode.SetActive(true);
        for (int i = 0; i < scoreArray.Length; ++i)
        {
            detailedScoreTexts[i].text = "Score: " + scoreArray[i];
        }
    }

    public void UpdateTopText(string newText)
    {
        topText.text = newText;
    }

    public void ResetDetailedScore()
    {
        detailedScoreNode.SetActive(false);
    }

    public void ShowNextRoundButton()
    {
        if (round < 6)
        {
            nextRoundButton.SetActive(true);
            round++;
        }


    }

}