using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BB_QuestRewardUIGroup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image backgroundImage;
    public Image rewardIcon;
    public TextMeshProUGUI rewardQuantity;
    public TextMeshProUGUI rewardName;
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
