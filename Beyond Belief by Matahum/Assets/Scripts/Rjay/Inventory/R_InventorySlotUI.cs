using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class R_InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image backdrop;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private R_InventoryItem currentItem;
    private R_InventoryItemInfoPanel infoPanel;

    [SerializeField] private Button inventoryItemButton;
    public R_InventoryUI inventoryUI;

    private Vector3 defaultScale = Vector3.one;
    private Vector3 selectedScale = Vector3.one * 1.1f; // for selection
    private Vector3 hoverScale = Vector3.one * 1.05f;   // for hover (slightly smaller than selected)

    [SerializeField] private GameObject equippedLabel;
    private Player player;

    private bool isSelected = false;
    private bool isHovered = false;

    // smoothing
    [SerializeField] private float scaleSpeed = 10f;
    private Vector3 targetScale;

    void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    void Start()
    {
        inventoryItemButton.onClick.AddListener(OnClickSlot);
        targetScale = defaultScale;
    }

    void Update()
    {
        // Smooth transition every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void Initialize(R_InventoryItemInfoPanel panel)
    {
        infoPanel = panel;
    }

    public void SetSlot(R_InventoryItem item)
    {
        currentItem = item;

        if (item != null && item.itemData != null)
        {
            if (backdrop != null)
            {
                backdrop.sprite = item.itemData.itemBackdropIcon;
                backdrop.enabled = item.itemData.itemBackdropIcon != null;
            }

            icon.sprite = item.itemData.itemIcon;
            icon.enabled = true;

            if (item.itemData.itemType == R_ItemType.Pamana)
                //quantityText.text = $"+{item.itemData.pamanaData.currentLevel}";
                quantityText.text = "1";
            else if (item.itemData.itemType == R_ItemType.Agimat)
                        quantityText.text = "1";
                    else if (item.itemData.isStackable)
                        quantityText.text = item.quantity.ToString();
                    else
                        quantityText.text = "1";
        }
        else
        {
            if (backdrop != null)
            {
                backdrop.sprite = null;
                backdrop.enabled = false;
            }

            icon.sprite = null;
            icon.enabled = false;
            quantityText.text = "";
        }

        if (equippedLabel != null)
        {
            bool isEquipped = false;

            if (item != null && item.itemData != null && item.itemData.itemType == R_ItemType.Pamana && player != null)
                isEquipped = player.IsPamanaEquipped(item);

            if (item != null && item.itemData != null && item.itemData.itemType == R_ItemType.Agimat && player != null)
                isEquipped = player.IsAgimatEquipped(item);

            equippedLabel.SetActive(isEquipped);
        }
    }

    public void OnClickSlot()
    {
        if (currentItem != null && currentItem.itemData != null)
        {
            infoPanel.ShowItem(currentItem.itemData);
            if (inventoryUI != null)
                inventoryUI.SetSelectedItem(currentItem);
        }
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
        return currentItem == item;
    }
}
