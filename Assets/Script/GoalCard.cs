
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GoalCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] GameObject textBackground;
    [SerializeField] GameObject gridImage;
    public float animationTime = 1.0f;
    public float scalingTime = 0.3f;
    public float spawnInterval = 0.1f;
    public GoalCardsMenu menu;
    public int methodIndex;
    public bool toChoose = false;
    public bool saved = false;
    bool isMouseIn = false;
    //public ParticleSystem vfx;
    

    public void Initialize(string title, string description, string points, Color background, Color txtBackground, Color wispColor, int mIndex)
    {
        titleText.text = title;
        descriptionText.text = description;
        pointsText.text = points;
        GetComponent<Image>().color = background;
        textBackground.GetComponent<Image>().color = txtBackground;
        //gridImage.GetComponent<RawImage>().color = wispColor;
        titleText.color = wispColor;
        methodIndex = mIndex;
       // transform.position += new Vector3(0, Screen.height / 2, 0);
        //toChoose = true;
        StartCoroutine(InitAnimation());
        //vfx.startColor = wispColor;
        //vfx.Play();


    }

    IEnumerator InitAnimation()
    {
        yield return new WaitForSeconds (methodIndex*spawnInterval);
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0, Screen.height / 2, 0);
        float counter = 0;
        transform.position = startPos;
        while(counter < animationTime / 2)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(2 * counter / animationTime);
            transform.position = Vector3.Lerp(startPos, endPos, t);
        }
        toChoose = true;
    }

    public void GetDown()
    {
        toChoose = false;
        StartCoroutine(GetingDown());
    }

    IEnumerator GetingDown()
    {
        yield return new WaitForSeconds(0.2f);
        float counter = 0;
        float time = animationTime;
        Vector3 startPos = transform.position;
        while (counter < time)
        {
            yield return null;
            transform.position = startPos - new Vector3(0, Screen.height * counter / time, 0);
            counter += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    public void GetUp()
    {
        toChoose = false;
        StartCoroutine(GetingUp());
        StartCoroutine(ScaleDown());
    }

    IEnumerator GetingUp()
    {
        float counter = 0;
        float time = animationTime;
        Vector3 startPos = transform.position;
        while (counter < time)
        {
            yield return null;
            transform.position = startPos + new Vector3(0, Screen.height * counter / time, 0);
            counter += Time.deltaTime;
        }
        //Destroy(gameObject);
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
        if (!toChoose) return;
        menu.GetOthersDown(gameObject);
        GetUp();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(toChoose) StartCoroutine(ScaleUp());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toChoose) StartCoroutine(ScaleDown());
    }

    public void WaitAndAddToSaved(float savedScale, Vector3 goalPosition)
    {
        StartCoroutine(AddToSaved(savedScale, goalPosition));
    }

    IEnumerator AddToSaved(float savedScale, Vector3 goalPosition)
    {
        yield return new WaitForSeconds(animationTime);
        Vector3 startPos = goalPosition + new Vector3(0, Screen.height / 2, 0);
        float counter = 0f;
        transform.localScale = new Vector3(savedScale, savedScale, savedScale);
        transform.position = startPos;
        while (counter < animationTime / 2)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / (animationTime / 2));
            transform.position = Vector3.Lerp(startPos, goalPosition, t);
        }
        menu.InitializeCards();
        saved = true;
    }

    IEnumerator ScaleUp()
    {
        isMouseIn = true;
        float counter = 0;
        Vector3 startScale = transform.localScale;
        Vector3 goalScale = new Vector3(1, 1, 1);
        while (counter < scalingTime && isMouseIn)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / scalingTime);
            transform.localScale = Vector3.Lerp(startScale, goalScale, t);
        }
    }
    IEnumerator ScaleDown()
    {
        isMouseIn = false;
        float counter = 0;
        Vector3 startScale = transform.localScale;
        Vector3 goalScale = new Vector3(menu.spawnScale, menu.spawnScale, menu.spawnScale);
        while (counter < scalingTime && !isMouseIn)
        {
            yield return null;
            counter += Time.deltaTime;
            float t = Mathf.Clamp01(counter / scalingTime);
            transform.localScale = Vector3.Lerp(startScale, goalScale, t);
        }
    }
}
