using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BB_IconUIGroup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image backgroundImage;
    public Image icon;
    public TextMeshProUGUI quantity;
    public TextMeshProUGUI iconName;
    public GameObject captionPanel;
    public TextMeshProUGUI captionPanelQuantity;

    public void OnPointerEnter(PointerEventData eventData)
    {
        captionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        captionPanel.SetActive(false);
    }
}
