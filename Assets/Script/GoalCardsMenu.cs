using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GoalCardsMenu : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    public Color[] wispColors;
    public Color[] backgroundColors;
    public Color[] textBackgorundColors;
    [SerializeField] GameObject spawningPoint;
    [SerializeField] GameObject savedPoint;
    public float spawnScale = 0.7f;
    public float savedScale = 0.4f;
    public float savedScaleBigger = 0.6f;
    public GameObject saveButton;
    public List<Texture2D> gridImages;
    public List<Sprite> pawsImages;
    public int[] cardsDifficulty = { 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 2};
    [SerializeField] string[] wispTitles;
    [SerializeField] string[] wispDescriptions;
    [SerializeField] TextMeshProUGUI wTitle;
    [SerializeField] TextMeshProUGUI wDescription;

    private int wispTypeIndex = 0;
    private List<GameObject> cardsList = new List<GameObject>();
    private List<GameObject> savedList = new List<GameObject>();
    private List<int> indexList = new List<int>();
    private GameObject lastEditedCard = null;

    public UI userInterface;


    void Start()
    {
        HideWispTexts();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitializeCards();
        }
    }

    public void InitializeCards(int againIndex = -1)
    {
       // for (int i = 0; i < cardsList.Count; ++i) Destroy(cardsList[i]);
        cardsList.Clear();
        if (savedList.Count >= 5 && againIndex == -1)
        {
            ScaleUpSavedCards();
            ShowSaveButton();
            ShowWispTexts(5);
            return;
        }
        int newIndex = (wispTypeIndex +1) % 5;
        if (againIndex != -1) newIndex = againIndex;
        else wispTypeIndex = (wispTypeIndex + 1) % 5;

        float spawnY = spawningPoint.GetComponent<RectTransform>().anchoredPosition.y - Screen.height/2;
        //spawnY = 0;
        int cardsAmount = newIndex == 4 ? 6 : 5;
        for (int i = 0; i < cardsAmount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            
            RectTransform rt = newCard.GetComponent<RectTransform>();
            rt.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = newIndex == 4 ? new Vector2(170 * i - 425, spawnY) : new Vector2(170 * i - 340, spawnY);

           // ShowWispTexts();

            newCard.transform.position = new Vector3(newCard.transform.position.x, spawningPoint.transform.position.y - Screen.height/2, newCard.transform.position.z);
            
            //newCard.transform.position = transform.position + new Vector3(200 * i -500, 400, 0);
            string[] infoArray = Score.GetInfoScoreMethods(newIndex + 1, i+1);
            newCard.GetComponent<GoalCard>().Initialize(infoArray[0], infoArray[1],
                (infoArray.Length==3) ? infoArray[2] : "", 
                backgroundColors[newIndex], textBackgorundColors[newIndex], wispColors[newIndex],
                i+1, gridImages[newIndex * 5 + i], pawsImages[cardsDifficulty[newIndex * 5 + i]], newIndex);
            cardsList.Add(newCard);
            newCard.GetComponent<GoalCard>().menu = this;
        }
    }

    public void ShowWispTexts(int wIndex)
    {
        wTitle.GetComponent<RectTransform>().anchoredPosition = new Vector3(wTitle.GetComponent<RectTransform>().anchoredPosition.x, wIndex == 5 ? -42: 22);
        wDescription.GetComponent<RectTransform>().anchoredPosition = new Vector3(wTitle.GetComponent<RectTransform>().anchoredPosition.x, wIndex == 5 ? -72 : -12);
        wTitle.gameObject.SetActive(true);
        wDescription.gameObject.SetActive(true);
        wTitle.color = wispColors[wIndex];
        wTitle.text = wispTitles[wIndex];
        wDescription.text = wispDescriptions[wIndex];
    }

    void HideWispTexts()
    {
        wTitle.gameObject.SetActive(false);
        wDescription.gameObject.SetActive(false);
    }


    public void GetOthersDown(GameObject upCard)
    {
        HideWispTexts();
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (cardsList[i] == upCard) continue;
            cardsList[i].GetComponent<GoalCard>().GetDown();
        }
        int betterIndex = (upCard.GetComponent<GoalCard>().wispType + 5 - 1) % 5;
        if (savedList.Count > betterIndex)
        {
            savedList[betterIndex] = upCard;
            indexList[betterIndex] = upCard.GetComponent<GoalCard>().methodIndex;
        }
        else
        {
            savedList.Add(upCard);
            indexList.Add(upCard.GetComponent<GoalCard>().methodIndex);
        }
        if (lastEditedCard != null) Destroy(lastEditedCard);
        upCard.GetComponent<GoalCard>().WaitAndAddToSaved(savedScale, new Vector3( Screen.width * (0.1f + 0.2f * betterIndex), savedPoint.transform.position.y, 0));
    }

    public void SaveMethods()
    {
        indexList.Insert(0, indexList[4]);
        indexList.RemoveAt(5);
        userInterface.SetScoreMethods(indexList);
        ResetMenu();
        gameObject.SetActive(false);
        
    }

    void ShowSaveButton()
    {
        saveButton.SetActive(true);
    }

    void ScaleUpSavedCards()
    {
        foreach (GameObject card in savedList)
        {
            card.GetComponent<GoalCard>().saved = false;
            card.GetComponent<GoalCard>().final = true;
            StartCoroutine(card.GetComponent<GoalCard>().ScaleUpSaved());
        }
    }

    public void ResetMenu()
    {
        foreach (GameObject card in cardsList) Destroy(card);
        foreach (GameObject card in savedList) Destroy(card);
        cardsList.Clear();
        savedList.Clear();
        indexList.Clear();
        wispTypeIndex = 0;
        saveButton.SetActive(false);
        HideWispTexts();

    }

    public void EditSaved( GameObject cardEdited)
    {
        
        foreach (GameObject card in cardsList)
        {
            card.GetComponent<GoalCard>().GetDown();
        }
        if (lastEditedCard == null)
        {
            wispTypeIndex = (wispTypeIndex + 5 - 1) % 5;
        }
        else
        {
            StartCoroutine(lastEditedCard.GetComponent<GoalCard>().GetingBackDown());
        }
        lastEditedCard = cardEdited;
        HideWispTexts();
    }

    public void EditFinal(GameObject cardEdited)
    {
        foreach (GameObject card in savedList)
        {
            //if (card == cardEdited) continue;
            card.GetComponent<GoalCard>().final = false;
            card.GetComponent<GoalCard>().saved = true;
            StartCoroutine(card.GetComponent<GoalCard>().ScaleDownSaved());
        }
        lastEditedCard = cardEdited;
        saveButton.SetActive(false);
        HideWispTexts();
    }
}
