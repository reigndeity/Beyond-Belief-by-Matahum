using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BB_ArchiveUITemplate : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public BB_ArchiveSO linkedArchiveSO;

    public Sprite undiscoveredSprite;
    public GameObject newDiscoverySprite;

    [Header("Properties for the Type of Archive")]
    public Vector2 normalTypeSize;
    public Vector2 locationTypeSize;
    public void Setup(BB_ArchiveSO archiveSO, Sprite defaultUndiscoveredSprite)
    {
        linkedArchiveSO = archiveSO;

        if (linkedArchiveSO.archiveType == ArchiveType.location) 
            GetComponent<RectTransform>().sizeDelta = locationTypeSize;    
        else 
            GetComponent<RectTransform>().sizeDelta = normalTypeSize;

        undiscoveredSprite = defaultUndiscoveredSprite;
        Refresh();
    }

    public void Refresh()
    {
        if (linkedArchiveSO == null) return;

        bool isDiscovered = PlayerPrefs.GetInt($"{linkedArchiveSO.archiveName}_Discovered", 0) == 1;
        if (isDiscovered)
        {
            nameText.text = linkedArchiveSO.archiveName;
            iconImage.sprite = linkedArchiveSO.archiveIcon;

            bool isViewed = PlayerPrefs.GetInt($"{linkedArchiveSO.archiveName}_Viewed", 0) == 1;
            newDiscoverySprite.SetActive(!isViewed);

        }
        else
        {
            nameText.text = "?????";
            iconImage.sprite = undiscoveredSprite;
        }
    }
}
