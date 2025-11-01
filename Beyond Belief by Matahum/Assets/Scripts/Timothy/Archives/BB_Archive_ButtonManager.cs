using UnityEngine;
using UnityEngine.UI;

public class BB_Archive_ButtonManager : MonoBehaviour
{
    [Header("Archive UI")]
    public GameObject archiveUI;

    [Header("Archive Category Button")]
    public BB_ArchivesCategoryButtonPopUp archivePopUp;
    public Button creatureCategoryButton;
    public Button locationCategoryButton;
    public Button wildlifeCategoryButton;
    public Button plantCategoryButton;

    [Header("Archive Panels")]
    public GameObject creaturePanel;
    public GameObject locationPanel;
    public Image locationImage;
    public GameObject wildlifePanel;
    public GameObject plantPanel;

    private void Start()
    {
        creatureCategoryButton.onClick.AddListener(OnOpenCreatureCategory);
        locationCategoryButton.onClick.AddListener(OnOpenLocationCategory);
        wildlifeCategoryButton.onClick.AddListener(OnOpenWildlifeCategory);
        plantCategoryButton.onClick.AddListener(OnOpenPlantCategory);
    }

    #region Open Archives
    public void OnOpenArchives()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);

        archiveUI.SetActive(true);

        OnOpenCreatureCategory();
    }
    #endregion
    #region Archive Categories
    public void OnOpenCreatureCategory()
    {
        locationImage.gameObject.SetActive(false);
        creaturePanel.SetActive(true);
        locationPanel.SetActive(false);
        wildlifePanel.SetActive(false);
        plantPanel.SetActive(false);

        archivePopUp.HighlightButton(creatureCategoryButton.GetComponent<RectTransform>());
        BB_ArchiveUI.instance.OnOpenJournal(BB_ArchiveUI.instance.creaturesScrollContent);
    }
    public void OnOpenLocationCategory()
    {
        locationImage.gameObject.SetActive(true);
        creaturePanel.SetActive(false);
        locationPanel.SetActive(true);
        wildlifePanel.SetActive(false);
        plantPanel.SetActive(false);

        archivePopUp.HighlightButton(locationCategoryButton.GetComponent<RectTransform>());
        BB_ArchiveUI.instance.OnOpenJournal(BB_ArchiveUI.instance.locationsScrollContent);

    }
    public void OnOpenWildlifeCategory()
    {
        creaturePanel.SetActive(false);
        locationPanel.SetActive(false);
        wildlifePanel.SetActive(true);
        plantPanel.SetActive(false);

        archivePopUp.HighlightButton(wildlifeCategoryButton.GetComponent<RectTransform>());
        BB_ArchiveUI.instance.OnOpenJournal(BB_ArchiveUI.instance.wildlifeScrollContent);
    }
    public void OnOpenPlantCategory()
    {
        locationImage.gameObject.SetActive(false);
        creaturePanel.SetActive(false);
        locationPanel.SetActive(false);
        wildlifePanel.SetActive(false);
        plantPanel.SetActive(true);

        archivePopUp.HighlightButton(plantCategoryButton.GetComponent<RectTransform>());
        BB_ArchiveUI.instance.OnOpenJournal(BB_ArchiveUI.instance.plantsScrollContent);
    }
    #endregion
    #region Exit Archives
    public void ExitArchives()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);

        archiveUI.SetActive(false);
        OnOpenCreatureCategory();
    }
    #endregion

    public bool IsArchiveOpen()
    {
        return archiveUI.activeSelf;
    }
}
