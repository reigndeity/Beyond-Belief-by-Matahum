using UnityEngine;
using UnityEngine.UI;

public class Shopkeep_ButtonManager : MonoBehaviour
{
    private Shopkeep shopkeep;
    [Header("Shop Category Filters")]
    public Button consumablesButton;
    public Button materialsButton;
    public Button agimatsButton;

    [Header("Buying Item")]
    public Button buy_ConfirmationButton;
    public Button cantBuyItemButton;
    public Button addQuantityButton;
    public Button minusQuantityButton;
    public Button buyButton;
    public Button cancelButton;

    [Header("Exit Button")]
    public Button exitButton;

    private void Start()
    {
        shopkeep = GetComponent<Shopkeep>();

        consumablesButton.onClick.AddListener(OnConsumableFilter);
        materialsButton.onClick.AddListener(OnMaterialFilter);
        agimatsButton.onClick.AddListener(OnAgimatFilter);

        buy_ConfirmationButton.onClick.AddListener(AttemptToBuy);
        addQuantityButton.onClick.AddListener(AddAmount);
        minusQuantityButton.onClick.AddListener(MinusAmount);
        buyButton.onClick.AddListener(BuyItem);
        cancelButton.onClick.AddListener (() => CloseConfirmationPanel(shopkeep.confirmationPanel));
        cantBuyItemButton.onClick.AddListener(() => CloseConfirmationPanel(shopkeep.cantBuyItemPanel));

        exitButton.onClick.AddListener(CloseShop);

    }
    #region SHOP FILTER
    void OnConsumableFilter()
    {
        shopkeep.OnOpenShopDetails(R_ItemType.Consumable);
    }

    void OnMaterialFilter()
    {
        shopkeep.OnOpenShopDetails(R_ItemType.UpgradeMaterial);
    }
    void OnAgimatFilter()
    {
        shopkeep.OnOpenShopDetails(R_ItemType.Agimat);
    }
    #endregion

    #region BUYING
    void AttemptToBuy()
    {
        shopkeep.AttemptToBuy();
    }

    void AddAmount()
    {
        shopkeep.AddAmount();
    }

    void MinusAmount()
    {
        shopkeep.MinusAmount();
    }

    void BuyItem()
    {
        shopkeep.BuyItem();
    }

    #endregion

    #region CLOSE PANELS
    void CloseShop()
    {
        shopkeep.shopkeeperUI.SetActive(false);
        HandleMouseVisibility(false);
    }

    void CloseConfirmationPanel(GameObject panel)
    {
        panel.SetActive(false);
        HandleMouseVisibility(true);
    }
    #endregion

    #region MOUSE VISIBILITY
    void HandleMouseVisibility(bool isVisible)
    {
        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(isVisible);
    }
    #endregion
}
