using UnityEngine;

[ExecuteAlways]
public class InfoPanelContentResizer : MonoBehaviour
{
    [Tooltip("The RectTransform of the Content inside the Scroll View")]
    [SerializeField] private RectTransform content;
    private void LateUpdate()
    {
        if (content == null) return;
        RecalculateContentHeight();
    }

    private void RecalculateContentHeight()
    {
        float lowestY = float.MaxValue;

        foreach (RectTransform child in content)
        {
            if (!child.gameObject.activeSelf) continue;

            float childBottom = child.anchoredPosition.y - child.rect.height;

            if (childBottom < lowestY)
                lowestY = childBottom;
        }

        // Make height positive and add padding
        float totalHeight = Mathf.Abs(lowestY);
        float padding = 1f; // Optional padding at bottom

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight + padding);
    }
}
