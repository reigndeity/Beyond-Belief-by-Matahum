using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

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
    public GameObject selectedNormalArchive;
    public GameObject selectedLocationArchive;
    public TextMeshProUGUI archiveTitleName;
    public Image archiveImage;
    public TextMeshProUGUI archiveDetailText;
    public Sprite undiscoveredLocationImage;
    public Sprite undiscoveredButtonImage;
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
                if (obj.archiveType == ArchiveType.location)
                    slotUI.Setup(localObj, undiscoveredLocationImage);
                else
                    slotUI.Setup(localObj, undiscoveredButtonImage);
            }

            Button archiveBtn = slotGO.GetComponent<Button>();
            archiveBtn.onClick.AddListener(() => ShowDetails(localObj, slotUI));

            archiveList.Add(slotGO);
        }
    }

    Transform currentScrollPane;
    public void OnOpenJournal(Transform scrollPanel) //This will auto select the first Quest when opening either the Journal, or switching between quest panels
    {
        if (currentScrollPane == scrollPanel) return;

        currentScrollPane = scrollPanel;

        selectedNormalArchive.SetActive(false);
        selectedLocationArchive.SetActive(false);

        if (scrollPanel.childCount > 0)
        {
            Button archiveBtn = scrollPanel.GetChild(0).GetComponent<Button>();
            archiveBtn?.onClick.Invoke();    
        }
    }

    public void ShowDetails(BB_ArchiveSO archiveSO, BB_ArchiveUITemplate uiTemplate)
    {
        if (PlayerPrefs.GetInt($"{archiveSO.archiveName}_Discovered", 0) == 0)
        {
            archiveTitleName.text = $"{undiscoveredName} {archiveSO.archiveType.ToString()}";
            archiveImage.sprite = undiscoveredLocationImage;
            archiveDetailText.text = $"{undiscoveredBodyText} {archiveSO.archiveType.ToString()}";
        }
        else
        {
            archiveTitleName.text = archiveSO.archiveName;
            archiveImage.sprite = archiveSO.archiveImage;
            archiveDetailText.text = archiveSO.archiveDescription;

            if (PlayerPrefs.GetInt($"{archiveSO.archiveName}_Viewed", 0) == 0) //0 = not viewed, 1 = viewed.
            {
                //archiveSO.isViewed = true;
                uiTemplate.newDiscoverySprite.SetActive(false);
                PlayerPrefs.SetInt($"{archiveSO.archiveName}_Viewed", 1);
                PlayerPrefs.Save();
            }
        }

        StartCoroutine(SetHighlightNextFrame(uiTemplate.transform, archiveSO));

    }

    private IEnumerator SetHighlightNextFrame(Transform target, BB_ArchiveSO archiveSO)
    {
        yield return null; // wait one frame for layout

        GameObject selectedBorder;

        if (archiveSO.archiveType == ArchiveType.location) selectedBorder = selectedLocationArchive;
        else selectedBorder = selectedNormalArchive;

        selectedBorder.SetActive(true);
        selectedBorder.transform.parent = target;
        selectedBorder.transform.position = target.position;
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
