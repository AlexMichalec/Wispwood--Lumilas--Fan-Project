using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;
using System.Collections.Generic;
using UnityEngine.InputSystem.Android;
public class UI : MonoBehaviour
{

    [Header("Menu")]
    public bool testingMenu = false;
    public GameObject MenuPanel;
    public GameObject StepOne;
    public GameObject CatTilePrefab;
    public Material[] CatMaterials;
    public Material[] CatMaterialsHidden;

    [Header("Scoring - UI")]
    public TextMeshProUGUI scoreText;
    public TMP_Dropdown[] scoreMethodDropdowns;
    public Color[] descriptionPanelColors;
    public Color basicPanelColor;
    public TextMeshProUGUI[] detailedScoreTexts;
    public TextMeshProUGUI[] descriptionTexts;
    public Image panelImage;
    public GameObject detailedScoreNode;

    [Header("Global - UI")]
    public TextMeshProUGUI catIsHiddenText;
    public TextMeshProUGUI topText;
    public GameObject nextRoundButton;
    public TextMeshProUGUI youCanMoveCatText;
    public int round = 4;

    [Header("Pond - UI")]
    public GameObject pondActions;
    public GameObject DealNewWispsButton;
    public GameObject CatActionWispsButton;
    public GameObject CatActionsShapesButton;

    [Header("Forest - UI")]
    public GameObject ArrowButtons;
    public GameObject ArrowForShapesButtons;
    public GameObject treeTurnActions;
    public TextMeshProUGUI treeCounter;
    private int treeCounterInt = 0;


    [Header("Navigation")]
    public GameManager gameManager;
    public GridManager gridManager;
    public Score scoreManager;
    public MoveCamera cameraMover;


    void Start()
    {
        scoreText.text = "Score: 0";
        InitializeScoreOptions();
        ShowScoreMethod(5);
        ResetDetailedScore();
        if (testingMenu)
        {
            StepOne.SetActive(true);
            MenuPanel.SetActive(true);
            cameraMover.changePosition();
        }
        
       // CatTilePrefab.transform.GetChild(0).GetComponent<Renderer>().material = CatMaterials[0];
        //CatTilePrefab.transform.GetChild(1).GetComponent<Renderer>().material = CatMaterialsHidden[0];



    }

    public void SetCatMaterial(int materialIndex)
    {
        if (CatMaterials.Length == 0) return;
        int index = materialIndex % CatMaterials.Length;
        int index2 = materialIndex % CatMaterialsHidden.Length;
        CatTilePrefab.transform.GetChild(0).GetComponent<Renderer>().material = CatMaterials[index];
        CatTilePrefab.transform.GetChild(1).GetComponent<Renderer>().material = CatMaterialsHidden[index2];
        gridManager.resetCat();
        gameManager.StartGame();
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

    public void flipCat()
    {
        catIsHiddenText.gameObject.SetActive(!catIsHiddenText.gameObject.activeSelf);
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

    public void HidePondActions()
    {
        pondActions.SetActive(false);
    }

    public void ShowPondActions()
    {
        CatActionWispsButton.SetActive(true);
        CatActionsShapesButton.SetActive(false);
        pondActions.SetActive(true);
        DealNewWispsButton.SetActive(gameManager.CanDealNewWispsForFree());
    }

    public void HideTreeTurnActions()
    {
        treeTurnActions.SetActive(false);
    }

    public void ShowTreeTurnActions()
    {
        treeCounter.text = "0/3";
        treeCounterInt = 0;
        treeTurnActions.SetActive(true);
    }

    public void TreeCounterUp()
    {
        treeCounterInt++;
        treeCounter.text = treeCounterInt + "/3";
    }

    public void ShowCatShapesButton()
    {
        CatActionWispsButton.SetActive(false);
        CatActionsShapesButton.SetActive(true);
    }
    public void ShowArrows()
    {
        ArrowButtons.SetActive(true);
        ArrowForShapesButtons.SetActive(false);
    }


    public void HideArrows()
    {
        ArrowButtons.SetActive(false);
    }

    public void ShowArrowsForShapes()
    {
        ArrowButtons.SetActive(true);
        ArrowForShapesButtons.SetActive(true);
    }

    public void ShowYouCanMoveCat()
    {
        youCanMoveCatText.gameObject.SetActive(true);
    }

    public void HideYouCanMoveCat()
    {
        youCanMoveCatText.gameObject.SetActive(false);
    }

    public void HideDealNewWisps()
    {
        DealNewWispsButton.SetActive(false);
    }


}