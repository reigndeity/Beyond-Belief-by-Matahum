using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BB_ArchiveUI : MonoBehaviour
{
    public static BB_ArchiveUI instance;

    [Header("Creatures Details")]
    public Transform creaturesScrollContent;

    [Header("Locations Details")]
    public Transform locationsScrollContent;

    [Header("Wildlife Details")]
    public Transform wildlifeScrollContent;

    [Header("Plants Details")]
    public Transform plantsScrollContent;

    [Header("Archive Slot Prefab and List")]
    public GameObject archiveSlotPrefab;
    public List<GameObject> archiveList = new List<GameObject>();

    [Header("Archive Details")]
    public TextMeshProUGUI archiveTitleName;
    public Image archiveImage;
    public TextMeshProUGUI archiveDetailText;
    public Sprite undiscoveredImage;
    private string undiscoveredName = "Undiscovered";
    private string undiscoveredBodyText = "You have not yet discovered this";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        InitialArchiveSetUp();
        BB_ArchiveManager.instance.OnArchiveUpdate += RefreshArchiveDisplay;
    }

    public void InitialArchiveSetUp()
    {
        BB_ArchiveManager.instance.SettingUpArchives();

        AddArchiveSlots(BB_ArchiveManager.instance.creatureList, creaturesScrollContent);
        AddArchiveSlots(BB_ArchiveManager.instance.locationList, locationsScrollContent);
        AddArchiveSlots(BB_ArchiveManager.instance.wildlifeList, wildlifeScrollContent);
        AddArchiveSlots(BB_ArchiveManager.instance.plantList, plantsScrollContent);
    }

    private void AddArchiveSlots(List<BB_ArchiveSO> archiveListSource, Transform parent)
    {
        foreach (var obj in archiveListSource)
        {
            var localObj = obj;
            GameObject slotGO = Instantiate(archiveSlotPrefab, parent);
            
            BB_ArchiveUITemplate slotUI = slotGO.GetComponent<BB_ArchiveUITemplate>();
            if (slotUI != null)
            {
                slotUI.Setup(localObj, undiscoveredImage);
            }

            Button archiveBtn = slotGO.GetComponent<Button>();
            archiveBtn.onClick.AddListener(() => ShowDetails(localObj, slotUI));

            archiveList.Add(slotGO);
        }
    }


    public void OnOpenJournal(Transform scrollPanel) //This will auto select the first Quest when opening either the Journal, or switching between quest panels
    {
        if (scrollPanel.childCount > 0)
        {
            Button archiveBtn = scrollPanel.GetChild(0).GetComponent<Button>();
            archiveBtn?.onClick.Invoke();
        }
    }

    public void ShowDetails(BB_ArchiveSO archiveSO, BB_ArchiveUITemplate uiTemplate)
    {
        if (!archiveSO.isDiscovered)
        {
            archiveTitleName.text = $"{undiscoveredName} {archiveSO.archiveType.ToString()}";
            archiveImage.sprite = undiscoveredImage;
            archiveDetailText.text = $"{undiscoveredBodyText} {archiveSO.archiveType.ToString()}";
        }
        else
        {
            archiveTitleName.text = archiveSO.archiveName;
            archiveImage.sprite = archiveSO.archiveImage;
            archiveDetailText.text = archiveSO.archiveDescription;

            if (!archiveSO.isViewed)
            {
                archiveSO.isViewed = true;
                uiTemplate.newDiscoverySprite.SetActive(false);
            }
        }
    }

    private void RefreshArchiveDisplay(BB_ArchiveSO ignoreThis)
    {
        foreach (var slotGO in archiveList)
        {
            BB_ArchiveUITemplate slotUI = slotGO.GetComponent<BB_ArchiveUITemplate>();
            if (slotUI != null)
            {
                slotUI.Refresh();
            }
        }
    }

    public IEnumerator HighlightCategory()
    {
        yield return new WaitForSeconds(10);
    }
}
