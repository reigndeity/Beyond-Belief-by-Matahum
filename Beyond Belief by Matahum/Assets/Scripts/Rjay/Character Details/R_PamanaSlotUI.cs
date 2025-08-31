using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class R_PamanaSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject equippedLabel;

    private R_InventoryItem representedItem;
    private R_PamanaPanel parentPanel;

    private Vector3 defaultScale = Vector3.one;
    private Vector3 hoverScale = Vector3.one * 1.05f;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    private bool isSelected = false;
    private bool isHovered = false;

    [SerializeField] private float scaleSpeed = 10f;
    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = defaultScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void Setup(R_InventoryItem item, R_PamanaPanel panel)
    {
        representedItem = item;
        parentPanel = panel;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.itemIcon;
            iconImage.enabled = true;

            backdropImage.sprite = item.itemData.itemBackdropIcon;
            backdropImage.enabled = backdropImage.sprite != null;

            int level = item.itemData.pamanaData?.currentLevel ?? 0;
            levelText.text = $"+{level}";

            bool isEquipped = panel.IsItemEquipped(item);
            equippedLabel.SetActive(isEquipped);
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => parentPanel.OnPamanaSelected(representedItem));
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateTargetScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateTargetScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateTargetScale();
    }

    private void UpdateTargetScale()
    {
        if (isSelected)
            targetScale = selectedScale;
        else if (isHovered)
            targetScale = hoverScale;
        else
            targetScale = defaultScale;
    }

    public bool RepresentsItem(R_InventoryItem item)
    {
        return representedItem == item;
    }
    public Button GetButton()
    {
        return selectButton;
    }
}
