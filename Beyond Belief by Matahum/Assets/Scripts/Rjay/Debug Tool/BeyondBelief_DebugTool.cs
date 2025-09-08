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

    void OnClickCloseDebugTool()
    {
        debugToolPanel.SetActive(false);
    }

    void OnClickLoadQuestID()
    {
        string questID = questIdInputField.text;
        if (!string.IsNullOrEmpty(questID))
        {
            BB_QuestManager.Instance.AcceptQuestByID(questID);
        }
    }

    void OnClickCompleteQuest()
    {
        BB_QuestManager.Instance.DebugCompleteAndClaimTrackedQuest();
    }

    void OnClickSaveProgress()
    {
        _ = SaveProgressAsync();
    }

    async Task SaveProgressAsync()
    {
        await GameManager.instance.SaveAll();
    }

    void OnClickLoadProgress()
    {
        _ = LoadProgressAsync();
    }

    async Task LoadProgressAsync()
    {
        await GameManager.instance.LoadAll();
    }

    void OnClickDeleteProgress()
    {
        GameManager.instance.DeleteAll();
    }

    void OnClickRestartGame()
    {
        Loader.Load(4);
    }

    void OnClickEnablePlayerAccess()
    { 
        TutorialManager.instance.AllowTemporaryBooleans();
    }
}
