using UnityEngine;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    public float scrollSpeed = 30f; // Pixels per second
    public RectTransform viewPort;

    private float startY;
    private float endY;
    private bool isScrolling = true;

    void Start()
    {
        startY = viewPort.anchoredPosition.y;
        endY = viewPort.rect.height;
    }

    void Update()
    {
        if (!isScrolling) return;

        viewPort.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (startY >= endY)
        {
            isScrolling = false;
            Invoke("BackToMenu", 2f); // Wait 2 seconds after credits finish
        }
    }

    void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
