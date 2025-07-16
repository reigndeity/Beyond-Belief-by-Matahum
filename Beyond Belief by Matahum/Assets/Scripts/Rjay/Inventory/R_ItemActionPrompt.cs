using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum R_ActionType { Use, Trash }

public class R_ItemActionPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private int currentAmount = 1;
    private int maxAmount = 1;
    private Action<int> onConfirm;

    public void Open(R_InventoryItem item, R_ActionType actionType, Action<int> confirmCallback)
    {
        gameObject.SetActive(true);
        currentAmount = 1;
        maxAmount = item.quantity;
        onConfirm = confirmCallback;

        itemNameText.text = item.itemData.itemName;
        iconImage.sprite = item.itemData.itemIcon;
        UpdateAmountText();

        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(() => ChangeAmount(1));

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(() => ChangeAmount(-1));

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => {
            onConfirm?.Invoke(currentAmount);
            Close();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Close);
    }

    private void ChangeAmount(int delta)
    {
        currentAmount = Mathf.Clamp(currentAmount + delta, 1, maxAmount);
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        quantityText.text = currentAmount.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
