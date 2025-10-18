using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float scaleUpFactor = 1.1f;   // how much bigger on hover
    [SerializeField] private float scaleSpeed = 10f;       // how fast it scales
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Smooth transition
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleUpFactor;
        AudioManager.instance.PlayOnHoverSFX();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}
