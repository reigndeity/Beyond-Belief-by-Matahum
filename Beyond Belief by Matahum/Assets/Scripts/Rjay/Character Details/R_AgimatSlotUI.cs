using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class R_AgimatSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private GameObject equippedLabel;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI quantityText;

    private R_InventoryItem representedItem;
    private R_AgimatPanel parentPanel;
    private Player player;

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
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void Setup(R_InventoryItem item, R_AgimatPanel panel)
    {
        representedItem = item;
        parentPanel = panel;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.itemIcon;
            iconImage.enabled = true;

            quantityText.text = "1";

            backdropImage.sprite = item.itemData.itemBackdropIcon;
            backdropImage.enabled = backdropImage.sprite != null;

            bool isEquipped = player != null && player.IsAgimatEquipped(item);
            equippedLabel.SetActive(isEquipped);
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => parentPanel.OnAgimatSelected(representedItem));
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
}
