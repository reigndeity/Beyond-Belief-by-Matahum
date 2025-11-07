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

    // General-purpose switch prompt
    public void Open(string message, Action confirmCallback)
    {
        gameObject.SetActive(true);

        messageText.text = message;
        onConfirm = confirmCallback;

        yesButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Close();
        });

        cancelButton.onClick.AddListener(Close);

        AudioManager.instance.PlayEquipSFX();
    }

    // Specific helper for Agimat messages
    public void OpenForAgimat(string selectedName, string equippedName, int fromSlot, int toSlot, 
                              bool isSameInstance, bool isSameType, Action confirmCallback)
    {
        string msg;

        if (isSameInstance)
        {
            // Case 1: Same instance, moving to another slot
            msg = $"{selectedName} is already equipped on Slot {fromSlot}. Do you want to equip it on Slot {toSlot} instead?";
        }
        else if (isSameType)
        {
            // Case 2: Another copy of the same Agimat type
            msg = $"The same type of agimat is already equipped on Slot {fromSlot}. " +
                  $"Do you want to replace it in this slot with this one instead?";
        }
        else
        {
            // Case 3: Different type replacing current
            msg = $"{equippedName} is currently equipped on Slot {toSlot}. " +
                  $"Replace it with {selectedName} instead?";
        }

        Open(msg, confirmCallback);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onConfirm = null;

        AudioManager.instance.PlayUnequipSFX();
    }
}
