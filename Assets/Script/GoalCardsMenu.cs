using UnityEngine;
using System.Collections.Generic;

public class GoalCardsMenu : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Color[] wispColors;
    [SerializeField] Color[] backgroundColors;
    [SerializeField] Color[] textBackgorundColors;
    private int wispTypeIndex = -1;
    private List<GameObject> cardsList = new List<GameObject>();


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

    void InitializeCards()
    {
        for (int i = 0; i < cardsList.Count; ++i) Destroy(cardsList[i]);
        cardsList.Clear();
        wispTypeIndex = (wispTypeIndex + 1) % 5;
        int cardsAmount = wispTypeIndex == 4 ? 6 : 5;
        for (int i = 0; i < cardsAmount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            
            RectTransform rt = newCard.GetComponent<RectTransform>();
            rt.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = wispTypeIndex == 4 ? new Vector2(180 * i - 450, 0) : new Vector2(180 * i - 360, 0);
            
            //newCard.transform.position = transform.position + new Vector3(200 * i -500, 400, 0);
            string[] infoArray = Score.GetInfoScoreMethods(wispTypeIndex + 1, i+1);
            newCard.GetComponent<GoalCard>().Initialize(infoArray[0], infoArray[1],
                (infoArray.Length==3) ? infoArray[2] : "", 
                backgroundColors[wispTypeIndex], textBackgorundColors[wispTypeIndex], wispColors[wispTypeIndex]);
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
    }
}
