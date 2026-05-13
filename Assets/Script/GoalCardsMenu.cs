using UnityEngine;
using System.Collections.Generic;

public class GoalCardsMenu : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Color[] wispColors;
    [SerializeField] Color[] backgroundColors;
    [SerializeField] Color[] textBackgorundColors;
    [SerializeField] GameObject spawningPoint;
    [SerializeField] GameObject savedPoint;
    public float spawnScale = 0.7f;
    public float savedScale = 0.4f;
    public float savedScaleBigger = 0.6f;
    public GameObject saveButton;
    [SerializeField] List<Texture2D> gridImages;
    [SerializeField] List<Sprite> pawsImages;
    int[] cardsDifficulty = { 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 2};

    private int wispTypeIndex = -1;
    private List<GameObject> cardsList = new List<GameObject>();
    private List<GameObject> savedList = new List<GameObject>();
    private List<int> indexList = new List<int>();

    public UI userInterface;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitializeCards();
        }
    }

    public void InitializeCards()
    {
       // for (int i = 0; i < cardsList.Count; ++i) Destroy(cardsList[i]);
        cardsList.Clear();
        if (wispTypeIndex == 4)
        {
            ShowSaveButton();
            return;
        }
        wispTypeIndex = (wispTypeIndex + 1) % 5;
        float spawnY = spawningPoint.GetComponent<RectTransform>().anchoredPosition.y - Screen.height/2;
        //spawnY = 0;
        int cardsAmount = wispTypeIndex == 4 ? 6 : 5;
        for (int i = 0; i < cardsAmount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            
            RectTransform rt = newCard.GetComponent<RectTransform>();
            rt.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = wispTypeIndex == 4 ? new Vector2(170 * i - 425, spawnY) : new Vector2(170 * i - 340, spawnY);
            

            newCard.transform.position = new Vector3(newCard.transform.position.x, spawningPoint.transform.position.y - Screen.height/2, newCard.transform.position.z);
            
            //newCard.transform.position = transform.position + new Vector3(200 * i -500, 400, 0);
            string[] infoArray = Score.GetInfoScoreMethods(wispTypeIndex + 1, i+1);
            newCard.GetComponent<GoalCard>().Initialize(infoArray[0], infoArray[1],
                (infoArray.Length==3) ? infoArray[2] : "", 
                backgroundColors[wispTypeIndex], textBackgorundColors[wispTypeIndex], wispColors[wispTypeIndex],
                i+1, gridImages[wispTypeIndex * 5 + i], pawsImages[cardsDifficulty[wispTypeIndex * 5 + i]]);
            cardsList.Add(newCard);
            newCard.GetComponent<GoalCard>().menu = this;
        }
    }

    public void GetOthersDown(GameObject upCard)
    {
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (cardsList[i] == upCard) continue;
            cardsList[i].GetComponent<GoalCard>().GetDown();
        }
        if(savedList.Count > wispTypeIndex)
        {
            savedList[wispTypeIndex] = upCard;
            indexList[wispTypeIndex] = upCard.GetComponent<GoalCard>().methodIndex;
        }
        else
        {
            savedList.Add(upCard);
            indexList.Add(upCard.GetComponent<GoalCard>().methodIndex);
        }
        upCard.GetComponent<GoalCard>().WaitAndAddToSaved(savedScale, savedPoint.transform.position + new Vector3( 160* wispTypeIndex,0,0));
    }

    public void SaveMethods()
    {
        userInterface.SetScoreMethods(indexList);
        gameObject.SetActive(false);
    }

    void ShowSaveButton()
    {
        saveButton.SetActive(true);
    }
}
