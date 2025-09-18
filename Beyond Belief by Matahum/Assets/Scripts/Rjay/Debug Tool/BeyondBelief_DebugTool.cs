using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeyondBelief_DebugTool : MonoBehaviour
{
    private Player m_player;

    [Header("Debug Properties")]
    public Button closeButton;
    public GameObject debugToolPanel;

    [Header("Quest Properties")]
    public TMP_InputField questIdInputField;
    public Button loadQuestButton;
    public Button completeQuestButton;

    [Header("Progress Properties")]
    public Button saveProgressButton;
    public Button loadProgressButton;
    public Button deleteProgressButton;
    public Button restartGameButton;

    [Header("Player Properties")]
    public Button infiniteHpButton;
    public Button infiniteDamageButton;
    public Button infiniteMoneyButton;
    public Button infiniteStaminaButton;
    public Button enablePlayerAcess;

    [Header("Misc Properties")]
    public Button resumeTimelineButton;
    public TMP_InputField debugLevelInputField; // 🆕 input for simulated level
    public Button spawnAgimatButton;
    public Button spawnPamanaButton;

    void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseDebugTool);

        loadQuestButton.onClick.AddListener(OnClickLoadQuestID);
        completeQuestButton.onClick.AddListener(OnClickCompleteQuest);

        questIdInputField.onValueChanged.AddListener(OnQuestIdChanged);
        OnQuestIdChanged(questIdInputField.text);

        saveProgressButton.onClick.AddListener(OnClickSaveProgress);
        loadProgressButton.onClick.AddListener(OnClickLoadProgress);
        deleteProgressButton.onClick.AddListener(OnClickDeleteProgress);
        restartGameButton.onClick.AddListener(OnClickRestartGame);

        enablePlayerAcess.onClick.AddListener(OnClickEnablePlayerAccess);

        resumeTimelineButton.onClick.AddListener(OnClickResumeTimeline);
        spawnAgimatButton.onClick.AddListener(OnClickSpawnAgimat);
        spawnPamanaButton.onClick.AddListener(OnClickSpawnPamana);

        // 🆕 apply simulated level when value changes
        debugLevelInputField.onEndEdit.AddListener(OnSimulatedLevelChanged);

        // set default at startup
        OnSimulatedLevelChanged(debugLevelInputField.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            debugToolPanel.SetActive(!debugToolPanel.activeSelf);
        }
    }

    private void OnQuestIdChanged(string value)
    {
        loadQuestButton.interactable = !string.IsNullOrEmpty(value);
    }

    void OnClickCloseDebugTool() => debugToolPanel.SetActive(false);

    void OnClickLoadQuestID()
    {
        string questID = questIdInputField.text;
        if (!string.IsNullOrEmpty(questID))
            BB_QuestManager.Instance.AcceptQuestByID(questID);
    }

    void OnClickCompleteQuest() => BB_QuestManager.Instance.DebugCompleteAndClaimTrackedQuest();

    public void OnClickSaveProgress() => _ = SaveProgressAsync();
    async Task SaveProgressAsync() => await GameManager.instance.SaveAll();

    void OnClickLoadProgress() => _ = LoadProgressAsync();
    async Task LoadProgressAsync() => await GameManager.instance.LoadAll();

    void OnClickDeleteProgress() => GameManager.instance.DeleteAll();
    void OnClickRestartGame() => Loader.Load(4);
    void OnClickEnablePlayerAccess() => TutorialManager.instance.AllowTemporaryBooleans();
    void OnClickResumeTimeline() => CutsceneManager.Instance.ResumeTimeline();
    void OnClickSpawnAgimat() => R_GeneralItemSpawner.instance.SpawnRandomAgimatFromTemplate();
    void OnClickSpawnPamana() => R_GeneralItemSpawner.instance.SpawnRandomPamanaFromTemplate();

    // 🆕 simulated level handler
    void OnSimulatedLevelChanged(string value)
    {
        int level = 1; // default fallback
        if (!string.IsNullOrEmpty(value))
        {
            if (!int.TryParse(value, out level))
                level = 1; // fallback if invalid
        }

        R_GeneralItemSpawner.instance.SetSimulatedLevel(level);
    }

}
