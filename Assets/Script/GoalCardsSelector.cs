using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GoalCardsSelector : MonoBehaviour
{
    public Toggle[] GoalSetsToggles;
    public TextMeshProUGUI[] GoalCardsTexts;
    public UI userInterface;
    public GameObject goalCardMenu;
    public GameObject settingsMenu;
    private int prevIndex = 0;
    private List<List<int>> cardsSets = new List<List<int>> { new List<int> { 1, 2, 3, 4, 5}, 
        new List<int> { 1, 2, 2, 2, 1 },  new List<int> { 3, 4, 3, 4, 3 },  new List<int> { 5, 5, 5, 6, 5 },  new List<int> { 0, 0, 0, 0, 0 } };


    void Start()
    {
        List<int> customSet = SaveSystem.LoadCustomSet();
        cardsSets[4] = customSet;

        if (PlayerPrefs.HasKey("CardSetIndex"))
        {
            
            SetByIndex(PlayerPrefs.GetInt("CardSetIndex"));
        }
        else 
        {
            SetRandom();
        }
        

    }



    public void SetRandom(bool fromUser = true)
    {
        if (!GoalSetsToggles[0].isOn) return;
        prevIndex = 0;
        if (fromUser) RandomizeSet();
        SetLabels(0);
    }

    public void SetEasy()
    {
        if (!GoalSetsToggles[1].isOn) return;
        prevIndex = 1;
        SetLabels(1);
    }

    public void SetMedium()
    {
        if (!GoalSetsToggles[2].isOn) return;
        prevIndex = 2;
        SetLabels(2);
    }

    public void SetHard()
    {
        if (!GoalSetsToggles[3].isOn) return;
        prevIndex = 3;
        SetLabels(3);
    }
    public void SetCustom()
    {
        if (!GoalSetsToggles[4].isOn) return;
        if (cardsSets[4][0] == 0)
        {
            goalCardMenu.SetActive(true);
            goalCardMenu.GetComponent<GoalCardsMenu>().InitializeCards();
            settingsMenu.SetActive(false);
            return;
        }
        prevIndex = 4;
        SetLabels(4);
    }

    void SetLabels(int setIndex)
    {
        for (int i = 0; i < 5; ++i)
        {
            int wispIndex = (i + 1) % 5 + 1;
            int methodIndex = cardsSets[setIndex][i];
            string[] wispNames = { "Dynie", "Serca", "Wiedźmy", "Ogniki", "Drzewa" };
            string[] infoArray = Score.GetInfoScoreMethods(wispIndex, methodIndex);
            GoalCardsTexts[i].text = wispNames[i] + ": " + infoArray[0];
            TooltipTrigger cardTooltip = GoalCardsTexts[i].gameObject.transform.GetComponentInParent<TooltipTrigger>();
            cardTooltip.tooltipHeader = infoArray[1];
            cardTooltip.tooltipContent = infoArray.Length > 2 ? Score.GetInfoScoreMethods(wispIndex, methodIndex)[2] : "";
        }
        SaveGoals();
    }

    void RandomizeSet()
    {
        for (int i = 0; i < 5; ++i) cardsSets[0][i] = Random.Range(1, i == 3 ? 7 : 6);
    }

    public void SetNewCustom(List<int> indexList)
    {
        
        for (int i = 0; i < 5; ++i)
        {
            cardsSets[4][(i+4)%5] = indexList[i];
        }
        settingsMenu.SetActive(true);
        GoalSetsToggles[4].isOn = true;
        SetCustom();
        SaveSystem.SaveCustomSet(cardsSets[4]);
    }

    public void BackFromCardMenu()
    {
        settingsMenu.SetActive(true);
        SetByIndex(prevIndex, false);
        
    }

    void SetByIndex(int index, bool changeRandom = true)
    {
        GoalSetsToggles[index].isOn = true;
        if (index == 0) SetRandom(changeRandom);
        if (index == 1) SetEasy();
        if (index == 2) SetMedium();
        if (index == 3) SetHard();
        if (index == 4) SetCustom();
    }

    public void SaveGoals()
    {
        PlayerPrefs.SetInt("CardSetIndex", prevIndex);
        List<int> resultList = new List<int>();
        for (int i = 0; i<5; ++ i)
        {
            resultList.Add(cardsSets[prevIndex][(i + 4)%5]);
        }
        userInterface.SetScoreMethods(resultList);
    }

    public void EditSet()
    {
        goalCardMenu.SetActive(true);
        goalCardMenu.GetComponent<GoalCardsMenu>().InitEdit(cardsSets[prevIndex]);
        settingsMenu.SetActive(false);
        
    }
}
