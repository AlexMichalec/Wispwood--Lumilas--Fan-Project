using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Score: 0";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
