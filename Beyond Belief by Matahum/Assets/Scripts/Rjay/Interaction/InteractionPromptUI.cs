using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image iconImage;
    public GameObject highlight;

    [HideInInspector] public Interactable target;

    public void Setup(Interactable interactable, bool isSelected)
    {
        target = interactable;
        itemNameText.text = interactable.interactName;
        iconImage.sprite = interactable.icon;
        highlight.SetActive(isSelected);
    }

    public void SetHighlight(bool value)
    {
        if (highlight.activeSelf != value)
            highlight.SetActive(value);
    }
}
