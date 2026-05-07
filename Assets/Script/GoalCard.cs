
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class GoalCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] GameObject textBackground;
    [SerializeField] GameObject gridImage;
    public GoalCardsMenu menu;

    public void Initialize(string title, string description, string points, Color background, Color txtBackground, Color wispColor)
    {
        titleText.text = title;
        descriptionText.text = description;
        pointsText.text = points;
        GetComponent<Image>().color = background;
        textBackground.GetComponent<Image>().color = txtBackground;
        gridImage.GetComponent<Image>().color = wispColor;
        titleText.color = wispColor;
    }

    public void GetDown()
    {
        StartCoroutine(GetingDown());
    }

    IEnumerator GetingDown()
    {
        float counter = 0;
        float time = 2.0f;
        Vector3 startPos = transform.position;
        while (counter < time)
        {
            yield return null;
            transform.position = startPos - new Vector3(0, 500 * counter / time, 0);
            counter += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    public void GetUp()
    {
        StartCoroutine(GetingUp());
    }

    IEnumerator GetingUp()
    {
        float counter = 0;
        float time = 2.0f;
        Vector3 startPos = transform.position;
        while (counter < time)
        {
            yield return null;
            transform.position = startPos + new Vector3(0, 500 * counter / time, 0);
            counter += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        print("KLIKAM");
        menu.GetOthersDown(gameObject);
        GetUp();
    }

    private void OnMouseEnter()
    {

        print("IN");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("KLIKAM1");
        menu.GetOthersDown(gameObject);
        GetUp();
    }
}
