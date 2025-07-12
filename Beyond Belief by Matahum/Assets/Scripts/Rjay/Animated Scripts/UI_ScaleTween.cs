using UnityEngine;

public class UIScaleTween : MonoBehaviour
{
    [Header("Scale Tween Settings")]
    public Vector2 startScale = Vector2.zero;
    public Vector2 endScale = Vector2.one;
    public float duration = 0.5f;
    public bool autoExpandOnStart = true;
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private float timer = 0f;
    private bool isAnimating = false;
    private Vector2 fromScale;
    private Vector2 toScale;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (autoExpandOnStart)
        {
            rectTransform.localScale = startScale;
            Expand();
        }
        else
        {
            rectTransform.localScale = fromScale = toScale = startScale;
        }
    }

    public void Expand()
    {
        fromScale = startScale;
        toScale = endScale;
        BeginTween();
    }

    public void Unexpand()
    {
        fromScale = endScale;
        toScale = startScale;
        BeginTween();
    }

    private void BeginTween()
    {
        timer = 0f;
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating)
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float easedT = easing.Evaluate(t);
        rectTransform.localScale = Vector2.Lerp(fromScale, toScale, easedT);

        if (t >= 1f)
            isAnimating = false;
    }
}
