using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;
using System.Collections.Generic;
public class UI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TMP_Dropdown[] scoreMethodDropdowns;
    public Color[] descriptionPanelColors;
    public Color basicPanelColor;
    public TextMeshProUGUI[] detailedScoreTexts;
    public TextMeshProUGUI[] descriptionTexts;
    public Image panelImage;
    //public GameObject scoreNode;
    //private Score scoreManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Score: 0";
        InitializeScoreOptions();
        
    }

    void InitializeScoreOptions()
    {
        
        for (int i = 0; i< scoreMethodDropdowns.Length; ++i)
        {
            scoreMethodDropdowns[i].ClearOptions();
            for (int j = 0; j < 6; ++j)
            {
                scoreMethodDropdowns[i].AddOptions(new List<string> { Score.GetInfoScoreMethods(i+1, j)[0] });
            }
            scoreMethodDropdowns[i].value = 1;
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
        string[] textsArray = Score.GetInfoScoreMethods(index+1, scoreMethodDropdowns[index].value);
        descriptionTexts[2].text = "";
        for (int i = 0; i<textsArray.Length; ++i)
        {
            descriptionTexts[i].text = Score.GetInfoScoreMethods(index+1, scoreMethodDropdowns[index].value)[i];
        }
        //descriptionTexts[1].text = Score.GetInfoScoreMethods(index, scoreMethodDropdowns[index + 1].value)[0];
        //descriptionTexts[1].text = Score.GetInfoScoreMethods(index, scoreMethodDropdowns[index+1].value)[1];

        
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
        for (int i = 0; i < scoreArray.Length; ++i)
        {
            detailedScoreTexts[i].text = "Score: " + scoreArray[i];
        }
    }
}
