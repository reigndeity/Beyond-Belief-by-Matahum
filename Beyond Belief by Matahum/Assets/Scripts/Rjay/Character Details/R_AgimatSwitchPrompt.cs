using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class R_AgimatSwitchPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;

    public void Open(string itemName, int fromSlot, int toSlot, Action confirmCallback)
    {
        gameObject.SetActive(true);
        messageText.text = $"{itemName} is equipped on Slot {fromSlot}. Equip it to Slot {toSlot} instead?";
        onConfirm = confirmCallback;

        yesButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => {
            onConfirm?.Invoke();
            Close();
        });

        cancelButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onConfirm = null;
    }
}
