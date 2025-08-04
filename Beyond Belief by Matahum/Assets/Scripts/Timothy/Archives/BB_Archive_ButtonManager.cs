using UnityEngine;
using UnityEngine.UI;

public class BB_Archive_ButtonManager : MonoBehaviour
{
    [Header("Archive UI")]
    public GameObject archiveUI;
    public Button openArchiveButton;

    [Header("Archive Category Button")]
    public BB_ArchivesCategoryButtonPopUp archivePopUp;
    public Button creatureCategoryButton;
    public Button locationCategoryButton;
    public Button wildlifeCategoryButton;
    public Button plantCategoryButton;

    [Header("Archive Panels")]
    public GameObject creaturePanel;
    public GameObject locationPanel;
    public GameObject wildlifePanel;
    public GameObject plantPanel;

    [Header("Exit Button")]
    public Button exitButton;

    private void Start()
    {
        openArchiveButton.onClick.AddListener(OnOpenArchives);
        creatureCategoryButton.onClick.AddListener(OnOpenCreatureCategory);
        locationCategoryButton.onClick.AddListener(OnOpenLocationCategory);
        wildlifeCategoryButton.onClick.AddListener(OnOpenWildlifeCategory);
        plantCategoryButton.onClick.AddListener(OnOpenPlantCategory);
        exitButton.onClick.AddListener(ExitArchives);
    }

    #region Open Archives
    public void OnOpenArchives()
    {
        archiveUI.SetActive(true);

        OnOpenCreatureCategory();
    }
    #endregion
    #region Archive Categories
    public void OnOpenCreatureCategory()
    {
        creaturePanel.SetActive(true);
        locationPanel.SetActive(false);
        wildlifePanel.SetActive(false);
        plantPanel.SetActive(false);

        archivePopUp.HighlightButton(creatureCategoryButton.GetComponent<RectTransform>());
        BB_ArchiveUI.instance.OnOpenJournal(BB_ArchiveUI.instance.creaturesScrollContent);
    }
    public void OnOpenLocationCategory()
    {
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
        archiveUI.SetActive(false);
    }
    #endregion
}
