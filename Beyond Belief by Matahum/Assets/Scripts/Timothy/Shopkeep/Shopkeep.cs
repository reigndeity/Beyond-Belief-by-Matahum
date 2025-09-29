using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;
using Unity.VisualScripting;

public class Shopkeep : MonoBehaviour
{
    [Header("References")]
    private PlayerStats playerStats;
    private R_Inventory inventory;
    public R_InventoryUI inventoryUI;
    public GameObject shopkeeperUI;

    [Header("For Item Spawning")]
    public GameObject itemGroupTemplatePrefab;
    public Transform shopItemSelectionContent;
    private List<Shopkeep_ItemGroupTemplate> spawnedItemTemplates = new List<Shopkeep_ItemGroupTemplate>();
    private bool isPopulated = false;


    [Header("Available Items in Shop")]
    [SerializeField] private List<R_ItemData> itemList = new List<R_ItemData>();

    [Header("Current Coin Count")]
    public TextMeshProUGUI coinAmount;

    [Header("Item Details")]
    public Shopkeep_ItemDetails itemDetails;
    public TextMeshProUGUI ownedItemStock;

    [Header("Buying Item")]
    [HideInInspector]public R_ItemData selectedItemData;
    public GameObject cantBuyItemPanel;
    public TextMeshProUGUI cantBuyItemText;
    public int buying_itemQuantity = 1;
    public int buying_totalAmountCost = 0;
    

    [Header("Confirmation")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmation_itemName;
    public Image confirmation_itemImage;
    public TextMeshProUGUI confirmation_itemQuantityText;
    public TextMeshProUGUI confirmation_totalAmountCostText;

    [Header("Item Selected Indicator")]
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    private Button currentlySelectedButton;
    private Image currentlySelectedImage;

    [Header("Item Restock Time")]
    public TextMeshProUGUI itemRestockTimerText;
    private DateTime nextRestockTime;
    public int restockIntervalHours = 100;
    private void Awake()
    {
        //Populate itemList with items with specific ItemType;
        var allowedTypes = new HashSet<R_ItemType>
        {
            R_ItemType.UpgradeMaterial,
            R_ItemType.Consumable,
            R_ItemType.Agimat
        };

        itemList = Resources.LoadAll<R_ItemData>("")
            .Where(item => allowedTypes.Contains(item.itemType))
            .ToList();
    }

    #region POPULATE BUTTONS
    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        inventory = FindFirstObjectByType<R_Inventory>();
        //inventoryUI = Resources.FindObjectsOfTypeAll<R_InventoryUI>().FirstOrDefault();

        foreach (var items in itemList)
        {
            if (items.itemType == R_ItemType.Agimat)
            {
                items.itemCost = 100;
            }
        }

        LoadOrCreateRestockTime();
        
    }

    private void PopulateButtons()
    {
        isPopulated = true;

        foreach (R_ItemData item in itemList)
        {
            Shopkeep_ItemGroupTemplate shopItemGroupTemplate = Instantiate(itemGroupTemplatePrefab, shopItemSelectionContent).GetComponent<Shopkeep_ItemGroupTemplate>();
            shopItemGroupTemplate.itemData = item;
            shopItemGroupTemplate.itemImage.sprite = item.itemIcon;
            shopItemGroupTemplate.itemName.text = item.itemName;
            shopItemGroupTemplate.itemCost.text = item.itemCost.ToString();

            int currentStock = PlayerPrefs.GetInt($"{item.itemName}_ShopStock", 100);
            shopItemGroupTemplate.itemCurrentStock.text = $"{currentStock}/100";

            spawnedItemTemplates.Add(shopItemGroupTemplate); // ✅ store reference

            Button shopItemBtn = shopItemGroupTemplate.GetComponent<Button>();
            Image shopItemBtnImg = shopItemBtn.GetComponent<Image>();

            shopItemBtn.onClick.AddListener(() => OnShopItemButtonClicked(shopItemGroupTemplate.itemData, shopItemBtn, shopItemBtnImg));
        }
    }


    private void UpdateItemStock()
    {
        foreach (var template in spawnedItemTemplates)
        {
            int currentStock = PlayerPrefs.GetInt($"{template.itemData.itemName}_ShopStock", 100);
            template.itemCurrentStock.text = $"{currentStock}/100";
        }
    }

    #endregion

    #region RESTOCK TIMER
    private void Update()
    {
        UpdateRestockTimer();
    }

    private void LoadOrCreateRestockTime()
    {
        if (PlayerPrefs.HasKey("NextRestockTime"))
        {
            nextRestockTime = DateTime.Parse(PlayerPrefs.GetString("NextRestockTime"));
        }
        else
        {
            nextRestockTime = DateTime.UtcNow.AddHours(restockIntervalHours);
            PlayerPrefs.SetString("NextRestockTime", nextRestockTime.ToString());
        }
    }

    private void UpdateRestockTimer()
    {
        TimeSpan remaining = nextRestockTime - DateTime.UtcNow;

        if (remaining.TotalSeconds <= 0)
        {
            RestockItems();
            nextRestockTime = DateTime.UtcNow.AddHours(restockIntervalHours);
            PlayerPrefs.SetString("NextRestockTime", nextRestockTime.ToString());
            remaining = nextRestockTime - DateTime.UtcNow;
        }

        int hours = Mathf.Max(0, (int)remaining.TotalHours);
        int minutes = Mathf.Max(0, remaining.Minutes);

        itemRestockTimerText.text = $"Item stock refreshes in: {hours} hour(s) {minutes} minute(s)";
    }

    private void RestockItems()
    {
        foreach (var item in itemList)
        {
            PlayerPrefs.SetInt($"{item.itemName}_ShopStock", 100); // Max stock
        }
        UpdateItemStock();
        Debug.Log("Shop restocked!");
    }
    #endregion

    #region SHOWING ITEM DETAILS
    public void OnOpenShopDetails(R_ItemType itemType)
    {
        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(true);

        coinAmount.text = playerStats.currentGoldCoins.ToString();

        if (!isPopulated) PopulateButtons();

        ChangeFilter(itemType);
    }
    private void OnShopItemButtonClicked(R_ItemData itemData, Button clickedButton, Image clickedImage)
    {
        // Reset previous button to default sprite
        if (currentlySelectedImage != null)
        {
            currentlySelectedImage.sprite = defaultSprite;
        }

        // Set new button sprite to selected
        currentlySelectedButton = clickedButton;
        currentlySelectedImage = clickedImage;
        currentlySelectedImage.sprite = selectedSprite;

        // Show quest details
        ShowItemDetails(itemData);
    }

    private void ShowItemDetails(R_ItemData itemData)
    {
        itemDetails.itemDescriptionText.text = "";

        if (itemData.itemType == R_ItemType.Consumable)
        {
            itemDetails.itemDescriptionText.text = $" • {itemData.effectText}\n\n";
        }

        itemDetails.itemIcon.sprite = itemData.itemIcon;
        itemDetails.itemName.text = itemData.itemName;
        itemDetails.itemTypeText.text = itemData.itemType.ToString();
        itemDetails.itemDescriptionText.text += $"{itemData.description}";
        itemDetails.itemCost.text = itemData.itemCost.ToString(); // should be itemData.itemCost
        itemDetails.itemHeaderImage.sprite = itemData.inventoryHeaderImage;
        itemDetails.itemBackdropImage.sprite = itemData.inventoryBackdropImage;
        ownedItemStock.text = LookForOwnedItems(itemData);

        selectedItemData = itemData;
    }

    string LookForOwnedItems(R_ItemData itemData)
    {
        int itemQuantity = 0;
        foreach (R_InventoryItem inventoryItem in inventory.items)
        {
            /*if (itemData == inventoryItem.itemData)
            {
                itemQuantity += inventoryItem.quantity;
            }*/

            if (itemData.itemName == inventoryItem.itemData.itemName)
            {
                itemQuantity += inventoryItem.quantity;
            }
        }

        return $"Owned: {itemQuantity}";
    }
    #endregion

    #region BUYING
    public void AttemptToBuy()
    {
        int currentStock = PlayerPrefs.GetInt($"{selectedItemData.itemName}_ShopStock", 100);

        if (playerStats.currentGoldCoins < selectedItemData.itemCost) 
        {
            ShowCantBuyPanel("Not enough gold");
            return;
        }
        else if (currentStock <= 0) // fixed stock check
        {
            ShowCantBuyPanel("No stock yet");
            return;
        }

        confirmationPanel.SetActive(true);
        confirmation_itemImage.sprite = selectedItemData.itemIcon;
        confirmation_itemName.text = selectedItemData.itemName;

        buying_itemQuantity = 1;
        UpdateBuyingCost();
    }

    public void AddAmount()
    {
        int nextQuantity = buying_itemQuantity + 1;
        int nextTotalCost = selectedItemData.itemCost * nextQuantity;

        if (nextTotalCost <= playerStats.currentGoldCoins)
        {
            buying_itemQuantity = nextQuantity;
            UpdateBuyingCost();
        }
    }

    public void MinusAmount()
    {
        if (buying_itemQuantity > 1)
        {
            buying_itemQuantity--;
            UpdateBuyingCost();
        }
    }

    public void BuyItem()
    {
        int totalCost = selectedItemData.itemCost * buying_itemQuantity;

        playerStats.currentGoldCoins -= totalCost;

        if (selectedItemData.itemType == R_ItemType.Agimat)
        {
            for(int i = 0; i < buying_itemQuantity; i++)
                R_GeneralItemSpawner.instance.SpawnSingleAgimat(new R_ItemData[] { selectedItemData });
        }
        else
        {
            inventory.AddItem(selectedItemData, buying_itemQuantity);
        }
        inventoryUI.RefreshUI();

        // Update stock
        int currentStock = PlayerPrefs.GetInt($"{selectedItemData.itemName}_ShopStock", 100);
        currentStock -= buying_itemQuantity;
        PlayerPrefs.SetInt($"{selectedItemData.itemName}_ShopStock", currentStock);

        // ✅ Refresh shop UI stock numbers
        UpdateItemStock();
        ownedItemStock.text = LookForOwnedItems(selectedItemData);

        // Update coin UI
        coinAmount.text = playerStats.currentGoldCoins.ToString();

        confirmationPanel.SetActive(false);
    }

    private void UpdateBuyingCost()
    {
        buying_totalAmountCost = selectedItemData.itemCost * buying_itemQuantity;
        confirmation_itemQuantityText.text = buying_itemQuantity.ToString();
        confirmation_totalAmountCostText.text = buying_totalAmountCost.ToString();
    }

    private void ShowCantBuyPanel(string message)
    {
        cantBuyItemPanel.SetActive(true);
        cantBuyItemText.text = message;
    }
    #endregion

    #region FILTER
    private void ChangeFilter(R_ItemType itemType)
    {
        foreach (var template in spawnedItemTemplates)
        {
            if (template.itemData.itemType == itemType) 
                template.gameObject.SetActive(true);
            else 
                template.gameObject.SetActive(false);
        }

        // Auto-select the first item in the new filter
        var firstActive = spawnedItemTemplates.FirstOrDefault(t => t.gameObject.activeSelf);
        if (firstActive != null)
        {
            Button firstBtn = firstActive.GetComponent<Button>();
            Image firstImg = firstBtn.GetComponent<Image>();
            OnShopItemButtonClicked(firstActive.itemData, firstBtn, firstImg);
        }

        UpdateItemStock();
    }
    #endregion
}
