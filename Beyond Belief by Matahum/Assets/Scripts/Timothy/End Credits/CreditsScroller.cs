using UnityEngine;
using TMPro;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    public float scrollSpeed = 30f; // Pixels per second
    private float currentSpeed;
    public RectTransform viewPort;

    private float startY;
    private float endY;
    private bool isScrolling = true;

    public UI_CanvasGroup canvasGroup;

    void Start()
    {
        if(canvasGroup != null)
            canvasGroup.FadeOut(0.5f);

        startY = viewPort.anchoredPosition.y;
        endY = viewPort.rect.height;
        currentSpeed = scrollSpeed;
    }

    void Update()
    {
        if (!isScrolling) return;

        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed = scrollSpeed * 5;
        }
        else currentSpeed = scrollSpeed;

        viewPort.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;

        if (viewPort.anchoredPosition.y >= endY)
        {
            isScrolling = false;
            StartCoroutine(BackToMenu());
        }
    }

    IEnumerator BackToMenu()
    {
        if (canvasGroup != null)
            canvasGroup.FadeIn(0.5f);

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
