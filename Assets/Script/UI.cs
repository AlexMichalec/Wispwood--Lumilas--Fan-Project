using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;
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
    public TextMeshProUGUI lastTurnText;
    public int round = 4;

    [Header("Pond - UI")]
    public GameObject pondActions;
    public GameObject dealNewWispsButton;
    public GameObject CatActionWispsButton;
    public GameObject CatActionsShapesButton;

    [Header("Forest - UI")]
    public GameObject ArrowButtons;
    public GameObject ArrowForShapesButtons;
    public GameObject treeTurnActions;
    public TextMeshProUGUI treeCounter;
    private int treeCounterInt = 0;

    [Header("Single Player")]
    public GameObject ghostButton;
    public TextMeshProUGUI ghostScoreboard;
    public TextMeshProUGUI ghostScoreboardRight;
    public GameObject ghostScoreWindow;
    public TextMeshProUGUI fireflyNowText;
    public TextMeshProUGUI fireflyLeftText;
    public GameObject fireFliesObject;
    public TextMeshProUGUI enemyActionInfo;
    public GameObject enemyActionObject;
    private List<string> scoreBoardStrings;
    private List<Vector2Int> scoreBoardList;
    private string[] wispNames = { "Dynia", "Serce", "Wiedźma", "Ognik" };



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
        dealNewWispsButton.SetActive(gameManager.CanDealNewWispsForFree());
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
        dealNewWispsButton.SetActive(false);
    }

    public IEnumerator InitializeGhostScoreboard(List<int> wispMultipliers)
    {
        List<Vector2Int> tempList = new List<Vector2Int>();
        for (int i = 0; i < wispMultipliers.Count; i++)
        {
            tempList.Add(new Vector2Int(wispMultipliers[i], i));
        }

        for (int i = 0; i < tempList.Count; ++i)
        {
            for (int j = i + 1; j < tempList.Count; ++j)
            {
                if (tempList[i].x < tempList[j].x)
                {
                    (tempList[i], tempList[j]) = (tempList[j], tempList[i]);
                }
            }
        }
        string goalBoard = "";
        scoreBoardStrings = new List<string>();
        for (int i = 0; i < tempList.Count; ++i)
        {
            goalBoard += tempList[i].x + ". " + wispNames[tempList[i].y] + "...........\n";
            scoreBoardStrings.Add(tempList[i].x + ". " + wispNames[tempList[i].y]);
        }
        //Animacja
        float counter = 0;
        float baseTime = 1f;
        int baseIndex = 0;
        string tempText = "";
        ghostScoreWindow.SetActive(true);

        while ( counter < baseTime)
        {
            yield return null;
            counter += Time.deltaTime;
            baseIndex = (baseIndex + 1) % wispNames.Length;
            tempText = "";
            for (int i = 0; i < tempList.Count; ++i)
            {
                int j = (baseIndex + i)%tempList.Count;
                tempText += tempList[i].x + ". " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;
        }

        while (counter < 2* baseTime)
        {
            yield return null;
            counter += Time.deltaTime;
            baseIndex = 1 + (baseIndex + 1) % (wispNames.Length-1);
            tempText = tempList[0].x + ". " + wispNames[tempList[0].y] + "\n";
            for (int i = 1; i < tempList.Count; ++i)
            {
                int j = 1 + (baseIndex + i) % (wispNames.Length - 1);
                tempText += tempList[i].x + ". " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;
        }

        while (counter < 3 * baseTime)
        {
            yield return null;
            counter += Time.deltaTime;
            baseIndex = 1 + (baseIndex + 1) % (wispNames.Length - 1);
            tempText = tempList[0].x + ". " + wispNames[tempList[0].y] + "\n" + tempList[1].x + ". " + wispNames[tempList[1].y] + "\n";
            for (int i = 2; i < tempList.Count; ++i)
            {
                int j = 1 + (baseIndex + i) % (wispNames.Length - 1);
                tempText += tempList[i].x + ". " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;
        }

        scoreBoardList = tempList;
        ghostScoreboard.text = goalBoard;
        
        ghostButton.SetActive(false);
        yield return new WaitForSeconds(2);
        ghostScoreWindow.SetActive(false);
        ghostButton.SetActive(true);
        StartCoroutine(gameManager.FirstEnemyMoveRound());
    }

    public void UpdateGhostScore(List<int> collectedWisps, int newWisp)
    {
        string result = "";
        for (int i = 0; i < scoreBoardList.Count; ++i)
        {
            int amount = collectedWisps[scoreBoardList[i].y];
            int points = amount * scoreBoardList[i].x;
            result += scoreBoardStrings[i] + " x" + amount + "\n"; //"....." + points + "p\n";
        }
        ghostScoreboard.text = result;

        StartCoroutine(EnemyActionUpdate("Duch zebrał: " + wispNames[newWisp]));
    }

    public void InitializeFireflies(int amount)
    {
        fireFliesObject.SetActive(true);
        fireflyNowText.text = "";
        fireflyLeftText.text = amount.ToString();
        StartCoroutine(EnemyActionUpdate("Przygotowano świetlików: " + amount));
    }

    public void UpdateFireflies(int now, int left)
    {
        fireflyNowText.text = now.ToString();
        fireflyLeftText.text = left.ToString();
        StartCoroutine(EnemyActionUpdate("Wylosowany Świetlik: " + now + " Pozostało świetlików: " + left, 3));
    }

    public IEnumerator EnemyActionUpdate(string newText, float lastTime = 2)
    {
        enemyActionObject.SetActive(true);
        enemyActionInfo.text = newText;
        yield return new WaitForSeconds(lastTime);
        enemyActionObject.SetActive(false);

    }

    public void ResetEnemyActionInfo()
    {
        enemyActionObject.SetActive(false);
    }

    public void ShowLastTurn()
    {
        lastTurnText.gameObject.SetActive(true);
    }

    public void HideLastTurn()
    {
        lastTurnText.gameObject.SetActive(false);
    }

    public IEnumerator ShowEnemyScoreRound(List <int> scoreArray)
    {
        ghostScoreWindow.SetActive(true);
        int runda = gameManager.round - 1;
        for (int i =0; i < 7; ++i)
        {
            yield return new WaitForSeconds(1);
            string tempText = "1   2   3";
            for (int j = 0;  j < scoreArray.Count; ++j)
            {
                if (j % 3 == 0) tempText += "\n";
                if (j % 3 < runda || (j % 3 == runda && j/3 < i) || ( i==6 && j == 15))
                {
                    int x = scoreArray[j];
                    if (x < 10)
                    {
                        tempText += "[ " + x + " ]";
                    }
                    else if (x < 100)
                    {
                        tempText += "[ " + x + "]";
                    }
                    else
                    {
                        tempText += "[" + x + "]";
                    }
                }
                else
                {
                    tempText += "[   ]";
                }
            }
            ghostScoreboardRight.text = tempText;

        }
        yield return new WaitForSeconds(3);
        ghostScoreWindow.SetActive(false);
    }
}