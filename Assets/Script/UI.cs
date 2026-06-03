using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    [Header("Menu")]
    public bool testingMenu = false;
    public GameObject MenuPanel;
    public GameObject StepOne;
    public GameObject CatTilePrefab;
    public GameObject enemyTilePrefab;
    public Material[] CatMaterials;
    public Material[] CatMaterialsHidden;
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyMultipliersText;
    public GameObject[] catsButtons;
    public TextMeshProUGUI ChooseCatTitle;
    public TextMeshProUGUI ChooseCatDesc;
    public Color ghostColor;
    public Color chooseColor;
    public float choosingGhostTime = 3.0f;
    public float ghostStepTime = 0.1f;
    public GameObject tutorialPanel;
    public TextMeshProUGUI highscoreText;
    private bool firstTime = true;
    private int catIndex;
    

    [Header("Scoring - UI")]
    public TextMeshProUGUI scoreText;
    public TMP_Dropdown[] scoreMethodDropdowns;
    public Color[] descriptionPanelColors;
    public Color basicPanelColor;
    public TextMeshProUGUI[] detailedScoreTexts;
    public TextMeshProUGUI[] descriptionTexts;
    public Image panelImage;
    public GameObject detailedScoreNode;
    public GoalCard goalCardGameplay;

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
    public GameObject PeekForestButton;

    [Header("Forest - UI")]
    public GameObject ArrowButtons;
    public GameObject ArrowForShapesButtons;
    public GameObject treeTurnActions;
    public TextMeshProUGUI treeCounter;
    public GameObject BackToPondButton;
    public GameObject undoArrow;
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
    private string[] wispNames = { "Dynie   ", "Serca   ", "Wiedźmy ", "Ogniki  " };
    private bool skipScoringAnimation = false;

    [Header("Game Over Screen")]
    public GameObject GameOverWindow;
    public GameObject GameOverWin;
    public GameObject GameOverLose;
    public GameObject GameOverTie;
    public TextMeshProUGUI GameOverScores;
    public TextMeshProUGUI highScoreGameOver;
    public TextMeshProUGUI highScoreBeaten;

    [Header("Navigation")]
    public GameManager gameManager;
    public GridManager gridManager;
    public Score scoreManager;
    public MoveCamera cameraMover;
    public EnemyManager enemyManager;
    public Tutorial tutorial;



    void Start()
    {
        scoreText.text = "Punkty: 0";
        int hiScore = SaveSystem.LoadHighscore();
        highscoreText.gameObject.SetActive(hiScore > 0);
        highscoreText.text = highscoreText.text + " " + hiScore;
        InitializeScoreOptions();
        ShowScoreMethod(5);
        ResetDetailedScore();
        fireFliesObject.SetActive(false);
        ghostButton.SetActive(false);
        if (testingMenu)
        {
            StepOne.SetActive(true);
            MenuPanel.SetActive(true);
            cameraMover.changePosition();
        }

        if (!Debug.isDebugBuild)
        {
            foreach (TMP_Dropdown dropdown in scoreMethodDropdowns)
            {
                dropdown.interactable = false;
                dropdown.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = new Vector2(-10,-7);
                dropdown.transform.GetChild(1).gameObject.SetActive(false);

            }

        }
        int x = SaveSystem.LoadHighscore();
        if (x == 0)
        {
            x = 4;
            SaveSystem.SaveHighscore(x);
            print("Brak Highscore");
        }
        else
        {
            print("HS wynosi: " + x);
        }

        if (PlayerPrefs.HasKey("CardSetIndex")) firstTime = false;

    }

    public void SetCatMaterial(int materialIndex)
    {
        if (firstTime && !gameManager.tutorialMode)
        {
            tutorialPanel.SetActive(true);
            catIndex = materialIndex;
            return;
        }
        if (CatMaterials.Length == 0) return;
        int index = materialIndex % CatMaterials.Length;
        CatTilePrefab.transform.GetChild(0).GetComponent<Renderer>().material = CatMaterials[index];
        CatTilePrefab.transform.GetChild(1).GetComponent<Renderer>().material = CatMaterialsHidden[index];
        StartCoroutine(ChooseGhostCatAnim(materialIndex));
    }

    public void ClickTutoYes()
    {
        gameManager.tutorialToggle.isOn = true;
        tutorialPanel.SetActive(false);
        firstTime = false;
        SetCatMaterial(catIndex);

    }

    public void ClickTutoNo()
    {
        gameManager.tutorialToggle.isOn = false;
        firstTime = false;
        tutorialPanel.SetActive(false);
        SetCatMaterial(catIndex);

    }

    public void ClickTutoCancel()
    {
        tutorialPanel.SetActive(false);

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
            scoreMethodDropdowns[i].value = UnityEngine.Random.Range(1, 6);
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

        goalCardGameplay.gameObject.SetActive(index < 5);
        if (index<5) goalCardGameplay.ShowDuringGame(index);



    }


    // Update is called once per frame
    void Update()
    {

    }

    public void ShowCatHidden()
    {
        catIsHiddenText.gameObject.SetActive(true);
        CatActionWispsButton.GetComponent<Button>().interactable = false;
        CatActionsShapesButton.GetComponent<Button>().interactable = false;
    }

    public void HideCatHidden()
    {
        catIsHiddenText.gameObject.SetActive(false);
        CatActionWispsButton.GetComponent<Button>().interactable = true;
        CatActionsShapesButton.GetComponent<Button>().interactable = true;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Punkty: " + score;
    }

    public void UpdateDetailedScore(int[] scoreArray)
    {
        detailedScoreNode.SetActive(true);
        for (int i = 0; i < scoreArray.Length; ++i)
        {
            detailedScoreTexts[i].text = "Punkty: " + scoreArray[i];
        }
        if (gameManager.tutorialMode && gameManager.round == 1) tutorial.Next();
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
        if (gameManager.tutorialMode && gameManager.round == 1) tutorial.Next();
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
        if (gameManager.tutorialMode && tutorial.tutIndex < 30) CatActionWispsButton.SetActive(false);
        else CatActionWispsButton.SetActive(true);
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
        undoArrow.GetComponent<Button>().interactable = !(gameManager.tutorialMode && tutorial.tutIndex < 28);
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
        if (!gameManager.tutorialMode || tutorial.tutIndex > 31) CatActionsShapesButton.SetActive(true);
    }
    public void ShowArrows(bool isCatTurn = false)
    {
        ArrowButtons.SetActive(true);
        undoArrow.SetActive(!isCatTurn && !(gameManager.tutorialMode && tutorial.tutIndex < 28));
        ArrowForShapesButtons.SetActive(false);
    }


    public void HideArrows()
    {
        ArrowButtons.SetActive(false);
    }

    public void ShowArrowsForShapes()
    {
        undoArrow.SetActive(!(gameManager.tutorialMode && tutorial.tutIndex < 28));
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
            goalBoard += tempList[i].x + "p. " + wispNames[tempList[i].y] + "...\n";
            scoreBoardStrings.Add(tempList[i].x + "p. " + wispNames[tempList[i].y]);
        }
        //Animacja
        float counter = 0;
        float baseTime = 0.8f;
        int baseIndex = 0;
        string tempText = "";
        skipScoringAnimation = gameManager.fastMode;
        ghostScoreWindow.SetActive(true);

        while ( counter < baseTime && !skipScoringAnimation)
        {
            yield return new WaitForSeconds(0.05f);
            counter += Time.deltaTime + 0.05f;
            baseIndex = (baseIndex + 1) % wispNames.Length;
            tempText = "";
            for (int i = 0; i < tempList.Count; ++i)
            {
                int j = (baseIndex + i)%tempList.Count;
                tempText += tempList[i].x + "p. " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;

        }

        if (gameManager.tutorialMode)
        {
            yield return new WaitForSeconds(0.5f);
            tutorial.Next();
        }

        while (counter < 2* baseTime && !skipScoringAnimation)
        {
            yield return new WaitForSeconds(0.05f);
            counter += Time.deltaTime + 0.05f;
            baseIndex = 1 + (baseIndex + 1) % (wispNames.Length-1);
            tempText = tempList[0].x + "p. " + wispNames[tempList[0].y] + "\n";
            for (int i = 1; i < tempList.Count; ++i)
            {
                int j = 1 + (baseIndex + i) % (wispNames.Length - 1);
                tempText += tempList[i].x + "p. " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;
        }

        while (counter < 3 * baseTime && !skipScoringAnimation)
        {
            yield return new WaitForSeconds(0.05f);
            counter += Time.deltaTime + 0.05f;
            baseIndex = 1 + (baseIndex + 1) % (wispNames.Length - 1);
            tempText = tempList[0].x + "p. " + wispNames[tempList[0].y] + "\n" + tempList[1].x + "p. " + wispNames[tempList[1].y] + "\n";
            for (int i = 2; i < tempList.Count; ++i)
            {
                int j = 1 + (baseIndex + i) % (wispNames.Length - 1);
                tempText += tempList[i].x + "p. " + wispNames[tempList[j].y] + "\n";
            }
            ghostScoreboard.text = tempText;
        }

        scoreBoardList = tempList;
        ghostScoreboard.text = goalBoard;
        
        ghostButton.SetActive(false);
        
        yield return new WaitForSeconds(2);
        ghostScoreWindow.SetActive(false);
        ghostButton.SetActive(true);
        skipScoringAnimation = false;
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
        if (newText == enemyActionInfo.text)
        enemyActionObject.SetActive(false);

    }

    public void ResetEnemyActionInfo()
    {
        enemyActionObject.SetActive(false);
    }

    public void ShowLastTurn()
    {
        if (gameManager.tutorialMode && gameManager.round == 1)
        {
            while (tutorial.tutIndex <= tutorial.treeTurnInfoIndex || tutorial.headers[tutorial.tutIndex] != "") ++tutorial.tutIndex;
            tutorial.Next();
        }
        lastTurnText.gameObject.SetActive(true);
        StartCoroutine(AnimateLastTurn());
    }

    public void HideLastTurn()
    {
        lastTurnText.gameObject.SetActive(false);
    }

    IEnumerator AnimateLastTurn()
    {
        while (lastTurnText.IsActive())
        {
            yield return new WaitForSeconds(0.5f);
            lastTurnText.color = Color.deepSkyBlue;
            yield return new WaitForSeconds(0.5f);
            lastTurnText.color = Color.white;
        }
        
    }

    public IEnumerator ShowEnemyScoreRound(List <int> scoreArray, int oldTotalSum)
    {
        skipScoringAnimation = false;
        yield return new WaitForSeconds(0.1f);

        ghostScoreWindow.SetActive(true);
        int runda = gameManager.round - 1;
        bool tutorialShowed = false;
        for (int i =0; i < 7; ++i)
        {

            yield return new WaitForSeconds(gameManager.fastMode ? 0.1f : 0.5f);
            if (gameManager.tutorialMode && gameManager.round == 1 && !tutorialShowed)
            {
                tutorialShowed = true;
                tutorial.Next();
            }
            if (skipScoringAnimation) i = 6;
            string tempText = "1    2    3";
            for (int j = 0;  j < scoreArray.Count; ++j)
            {
                if (j % 3 == 0) tempText += "\n";
                if (j % 3 < runda || (j % 3 == runda && j/3 < i) || ( i==6 && j == 15))
                {
                    int x = scoreArray[j];
                    if (j == 15 & i != 6) x = oldTotalSum;
                    if (x < 10)
                    {
                        tempText += "[  " + x + "]";
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
        if (!skipScoringAnimation)
        {
            yield return new WaitForSeconds(2);
            ghostScoreWindow.SetActive(false);
            yield return new WaitForSeconds(1);
        }
        skipScoringAnimation = false;
        if (gameManager.round == 3) gameManager.GameOver();
        else gameManager.PrepareCatToMove();
    }

    public void ShowGameOverWindow(int myScore, int enemyScore)
    {
        GameOverWin.SetActive(myScore > enemyScore);
        GameOverLose.SetActive(myScore < enemyScore);
        GameOverTie.SetActive(myScore == enemyScore);
        GameOverScores.text = myScore + "p. vs " + enemyScore + "p.";
        GameOverWindow.SetActive(true);
        if (myScore > SaveSystem.LoadHighscore())
        {

            StartCoroutine(UpdateHighScore(SaveSystem.LoadHighscore(), myScore));
            SaveSystem.SaveHighscore(myScore);
            highScoreBeaten.gameObject.SetActive(true);
        }
        else
        {
            highScoreBeaten.gameObject.SetActive(false);
            highScoreGameOver.text += " " + SaveSystem.LoadHighscore();
        }
    }

    IEnumerator UpdateHighScore(int before, int now)
    {
        string baseText = highScoreGameOver.text;

        highScoreGameOver.text = baseText + " " + before;
        yield return new WaitForSeconds(1);
        while (before < now)
        {
            yield return null;
            before += 1;
            highScoreGameOver.text = baseText + " " + before;
        }

    }

    public void PeekYourForest()
    {
        HidePondActions();
        cameraMover.changePosition();
        Invoke("ShowBackButton", 1f);
    }

    public void GoBackToPond()
    {
        BackToPondButton.SetActive(false);
        cameraMover.changePosition();
        Invoke("ShowPondActions", 1f);
    }

    public void ShowBackButton()
    {
        BackToPondButton.SetActive(true);
    }

    public void UpdateWispsMultipliers()
    {
        enemyManager.difficultyLevel = (int) difficultySlider.value;
        string[] textArray = {"6, 5, 4, 3", "8, 6, 5, 3", "8, 7, 5, 4", "8, 7, 6, 5"};
        difficultyMultipliersText.text = textArray[(int) difficultySlider.value];
    }

    public void SkipEnemyScoreAnimation()
    {
        skipScoringAnimation = true;
    }

    public void SetScoreMethods(List<int> methodsList)
    {
        //foreach (int x in methodsList) print("P" + x);
        for (int i = 0; i < methodsList.Count; ++i)
        {
            scoreMethodDropdowns[i].value = methodsList[i];
            scoreManager.UpdateScoreMethod(i, methodsList[i]);
        }
    }

    public void HideUndoArrow()
    {
        undoArrow.SetActive(false);
    }

    IEnumerator ChooseGhostCatAnim(int chosenCatIndex)
    {
        foreach (GameObject cat in catsButtons)
        {
            cat.GetComponent<Button>().interactable = false;
        }

        float time = choosingGhostTime;
        float counter = 0;
        int prevIndex = -1;
        Color startColor = catsButtons[0].GetComponent<Image>().color;

        catsButtons[chosenCatIndex].GetComponent<Image>().color = chooseColor;

        ChooseCatTitle.text = "";
        ChooseCatDesc.text = "Losuję Kota - Widmo";

        
        while (counter < time)
        {
            yield return new WaitForSeconds(ghostStepTime);
            counter += Time.deltaTime + ghostStepTime;
            if (gameManager.fastMode) counter = time;
            int i = Random.Range(0, 12);
            while (i == chosenCatIndex || i == prevIndex) i = Random.Range(0, 12);
            if (prevIndex != -1) catsButtons[prevIndex].GetComponent<Image>().color = startColor;
            catsButtons[i].GetComponent<Image>().color = ghostColor;
            prevIndex = i;
        }
        
        for (int i = 0; i < 2; ++i)
        {
            yield return new WaitForSeconds(gameManager.fastMode ? ghostStepTime : 2 * ghostStepTime);
            catsButtons[prevIndex].GetComponent<Image>().color = startColor;
            yield return new WaitForSeconds(gameManager.fastMode ? ghostStepTime : 2 * ghostStepTime);
            catsButtons[prevIndex].GetComponent<Image>().color = ghostColor;
        }
        yield return new WaitForSeconds(gameManager.fastMode ? ghostStepTime : 2 * ghostStepTime);

        int indexEnemy = prevIndex;
        enemyTilePrefab.transform.GetChild(0).GetComponent<Renderer>().material = CatMaterials[indexEnemy];
        enemyTilePrefab.transform.GetChild(1).GetComponent<Renderer>().material = CatMaterialsHidden[indexEnemy];
        gridManager.resetCat();
        gameManager.StartGame();
        MenuPanel.SetActive(false);
        
    }
    
}