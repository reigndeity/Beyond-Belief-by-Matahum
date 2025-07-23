using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class R_AgimatPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private R_Inventory playerInventory;
    [SerializeField] private Player player;
    [SerializeField] private R_AgimatSwitchPrompt switchPrompt;

    [Header("Slot Buttons")]
    [SerializeField] private Button slot1Button;
    [SerializeField] private Button slot2Button;

    [Header("Slot Highlights")]
    [SerializeField] private GameObject highlightSlot1;
    [SerializeField] private GameObject highlightSlot2;

    [Header("Equipped Icons")]
    [SerializeField] private Image iconSlot1;
    [SerializeField] private Image iconSlot2;

    [Header("Inventory List")]
    [SerializeField] private Transform agimatListParent;
    [SerializeField] private GameObject agimatSlotPrefab;

    [Header("Info Panel")]
    [SerializeField] private R_InfoPanel_Agimat infoPanel;

    [Header("Action Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private List<R_InventoryItem> agimatItems = new();
    private List<GameObject> slotUIObjects = new();

    private R_InventoryItem selectedItem;
    private int? selectedSlot = null;

    private void Start()
    {
        slot1Button.onClick.AddListener(() => OnSelectSlot(1));
        slot2Button.onClick.AddListener(() => OnSelectSlot(2));

        equipButton.onClick.AddListener(OnClickEquip);
        unequipButton.onClick.AddListener(OnClickUnequip);
    }

    private void OnEnable()
    {
        selectedItem = null;
        selectedSlot = null;

        equipButton.interactable = false;
        unequipButton.interactable = false;

        agimatListParent.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);

        UpdateSlotIcons();
        UpdateSlotHighlight();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedItem = null;
            selectedSlot = null;

            agimatListParent.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);

            equipButton.interactable = false;
            unequipButton.interactable = false;

            UpdateSlotHighlight();
            RefreshAgimatList();
        }
    }

    private void OnSelectSlot(int slot)
    {
        selectedSlot = slot;
        agimatListParent.gameObject.SetActive(true);
        RefreshAgimatList();
        UpdateSlotHighlight();

        var equipped = player.GetEquippedAgimat(slot);

        if (equipped != null)
        {
            selectedItem = equipped;
            infoPanel.gameObject.SetActive(true);
            infoPanel.Show(equipped.itemData);
        }
        else
        {
            selectedItem = null;
            infoPanel.gameObject.SetActive(false);
        }

        UpdateSelectionVisuals();
        UpdateInfoPanelAndButtons();
    }

    private void RefreshAgimatList()
    {
        agimatItems.Clear();

        foreach (var item in playerInventory.items)
        {
            if (item.itemData != null && item.itemData.itemType == R_ItemType.Agimat)
                agimatItems.Add(item);
        }

        foreach (var obj in slotUIObjects)
            Destroy(obj);
        slotUIObjects.Clear();

        foreach (var item in agimatItems)
        {
            GameObject slotObj = Instantiate(agimatSlotPrefab, agimatListParent);
            var slotUI = slotObj.GetComponent<R_AgimatSlotUI>();
            slotUI.Setup(item, this);
            slotUIObjects.Add(slotObj);
        }

        UpdateSelectionVisuals();
    }

    public void OnAgimatSelected(R_InventoryItem item)
    {
        selectedItem = item;

        if (item != null && selectedSlot != null)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.Show(item.itemData);
        }
        else
        {
            infoPanel.gameObject.SetActive(false);
        }

        UpdateInfoPanelAndButtons();
        UpdateSelectionVisuals();
    }
    private void OnClickEquip()
    {
        if (selectedItem == null || selectedSlot == null)
            return;

        int currentSlot = selectedSlot.Value;
        int otherSlot = currentSlot == 1 ? 2 : 1;

        var equippedInCurrent = player.GetEquippedAgimat(currentSlot);
        var equippedInOther = player.GetEquippedAgimat(otherSlot);

        bool isSelectedEquippedInCurrent = equippedInCurrent != null && 
                                        equippedInCurrent.runtimeID == selectedItem.runtimeID;

        bool isSelectedEquippedInOther = equippedInOther != null && 
                                        equippedInOther.runtimeID == selectedItem.runtimeID;

        // 🟡 CASE 1: Trying to re-equip the same Agimat in the same slot
        if (isSelectedEquippedInCurrent)
        {
            Debug.Log("This Agimat is already equipped in the selected slot.");
            return;
        }

        // 🔵 CASE 2: Trying to switch equipped Agimat to another slot (desired prompt)
        if (isSelectedEquippedInOther)
        {
            string itemName = selectedItem.itemData.itemName;
            switchPrompt.Open(itemName, otherSlot, currentSlot, () => {
                player.UnequipAgimat(otherSlot);
                player.EquipAgimat(selectedItem, currentSlot);
                infoPanel.Show(selectedItem.itemData);
                UpdateSlotIcons();
                UpdateInfoPanelAndButtons();
                UpdateSelectionVisuals();
                RefreshAgimatList();
            });
            return;
        }

        // 🔴 CASE 3: Trying to equip a DIFFERENT Agimat with the same name → reject
        if (equippedInCurrent != null && equippedInCurrent.itemData.name == selectedItem.itemData.name)
        {
            Debug.Log("A different copy of this Agimat is already equipped. Select the equipped one to move it.");
            return;
        }

        // ✅ Prevent equipping two of the same named Agimat in both slots
        bool duplicateNameEquippedElsewhere =
            equippedInOther != null &&
            equippedInOther.itemData.name == selectedItem.itemData.name &&
            equippedInOther.runtimeID != selectedItem.runtimeID;

        if (duplicateNameEquippedElsewhere)
        {
            // ADD VISUAL POP UP LATER
            Debug.Log("You cannot equip two of the same Agimat.");
            return;
        }

        // ✅ Equip normally
        player.EquipAgimat(selectedItem, currentSlot);
        infoPanel.Show(selectedItem.itemData);
        UpdateSlotIcons();
        UpdateInfoPanelAndButtons();
        UpdateSelectionVisuals();
        RefreshAgimatList();
        
    }


    private void OnClickUnequip()
    {
        if (selectedSlot == null)
            return;

        int slot = selectedSlot.Value;
        player.UnequipAgimat(slot);

        selectedItem = null;
        infoPanel.gameObject.SetActive(false);

        UpdateSlotIcons();
        UpdateInfoPanelAndButtons();
        UpdateSelectionVisuals();
        RefreshAgimatList();
    }

    private void UpdateSlotIcons()
    {
        SetIcon(iconSlot1, player.GetEquippedAgimat(1));
        SetIcon(iconSlot2, player.GetEquippedAgimat(2));
    }

    private void SetIcon(Image target, R_InventoryItem item)
    {
        if (item != null && item.itemData != null)
        {
            target.sprite = item.itemData.itemIcon;
            target.enabled = true;
        }
        else
        {
            target.sprite = null;
            target.enabled = false;
        }
    }

    private void UpdateSlotHighlight()
    {
        highlightSlot1.SetActive(selectedSlot == 1);
        highlightSlot2.SetActive(selectedSlot == 2);
    }

    private void UpdateInfoPanelAndButtons()
    {
        if (selectedSlot != null && selectedItem != null)
        {
            equipButton.interactable = true;
            equipButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Equip to Slot {selectedSlot}";
        }
        else
        {
            equipButton.interactable = false;
        }

        if (selectedSlot != null)
        {
            var equipped = player.GetEquippedAgimat(selectedSlot.Value);
            bool isSameInstance = equipped != null && equipped == selectedItem;
            unequipButton.interactable = isSameInstance;
        }
        else
        {
            unequipButton.interactable = false;
        }

    }

    private void UpdateSelectionVisuals()
    {
        foreach (var obj in slotUIObjects)
        {
            if (obj.TryGetComponent<R_AgimatSlotUI>(out var slotUI))
                slotUI.SetSelected(slotUI.RepresentsItem(selectedItem));
        }
    }
}
