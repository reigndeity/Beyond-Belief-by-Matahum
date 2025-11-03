using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeyondBelief_DebugTool : MonoBehaviour
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private PlayerMovement m_playerMovement;

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
    private bool infiniteHealth = false;
    public Button infiniteDamageButton;
    private bool infiniteDamage = false;
    public Button infiniteMoneyButton;
    public Button infiniteStaminaButton;
    private bool infiniteStamina = false;
    public Button enablePlayerAcess;

    [Header("Misc Properties")]
    public Button resumeTimelineButton;
    public TMP_InputField debugLevelInputField; // 🆕 input for simulated level
    public Button spawnAgimatButton;
    public Button spawnPamanaButton;
    public Button ghostWalkButton;
    private bool isGhostMode = false;
    [SerializeField] CharacterController playerController;

    void Awake()
    {
        m_player = FindFirstObjectByType<Player>();
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_playerMovement = FindFirstObjectByType<PlayerMovement>();
    }
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

        infiniteHpButton.onClick.AddListener(OnClickInfiniteHealth);
        infiniteDamageButton.onClick.AddListener(OnClickInfiniteDamage);
        infiniteMoneyButton.onClick.AddListener(OnClickInfiniteMoney);
        infiniteStaminaButton.onClick.AddListener(OnClickInfiniteStamina);
        enablePlayerAcess.onClick.AddListener(OnClickEnablePlayerAccess);

        resumeTimelineButton.onClick.AddListener(OnClickResumeTimeline);
        spawnAgimatButton.onClick.AddListener(OnClickSpawnAgimat);
        spawnPamanaButton.onClick.AddListener(OnClickSpawnPamana);
        ghostWalkButton.onClick.AddListener(OnClickGhostWalk);

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
            if (debugToolPanel.activeSelf)
            {
                PlayerCamera.Instance.SetCursorVisibility(true);
            }
            else
            {
                PlayerCamera.Instance.SetCursorVisibility(false);
            }
        }
    }

    private void OnQuestIdChanged(string value)
    {
        loadQuestButton.interactable = !string.IsNullOrEmpty(value);
    }

    void OnClickCloseDebugTool()
    { 
        debugToolPanel.SetActive(false);
        PlayerCamera.Instance.SetCursorVisibility(false);    
    }

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
    void OnClickSpawnAgimat() => R_GeneralItemSpawner.instance.DebugSpawnRandomAgimat();
    void OnClickSpawnPamana() => R_GeneralItemSpawner.instance.DebugSpawnRandomPamana();
    void OnClickGhostWalk()
    {
        if (isGhostMode == false)
        {
            ghostWalkButton.GetComponent<Image>().color = Color.green;
            isGhostMode = true;
            playerController.excludeLayers = LayerMask.GetMask("Building", "Stone", "Fence", "Tree");
        }
        else
        {
            ghostWalkButton.GetComponent<Image>().color = Color.white;
            isGhostMode = false;
            playerController.excludeLayers = 0;
        }
    }

    void OnClickInfiniteHealth()
    {
        infiniteHealth = !infiniteHealth;

        if (infiniteHealth)
        {
            infiniteHpButton.GetComponent<Image>().color = Color.green;
            m_playerStats.p_currentHealth = m_playerStats.p_maxHealth;
            StartCoroutine(KeepHealthFull());
        }
        else
        {
            infiniteHpButton.GetComponent<Image>().color = Color.white;
            StopCoroutine(KeepHealthFull());
        }
    }

    IEnumerator KeepHealthFull()
    {
        while (infiniteHealth)
        {
            m_playerStats.p_currentHealth = m_playerStats.p_maxHealth;
            yield return null;
        }
    }
    void OnClickInfiniteDamage()
    {
        infiniteDamage = !infiniteDamage;

        if (infiniteDamage)
        {
            infiniteDamageButton.GetComponent<Image>().color = Color.green;
            m_playerStats.p_attack = int.MaxValue; // basically one-shot
        }
        else
        {
            infiniteDamageButton.GetComponent<Image>().color = Color.white;
            m_playerStats.RecalculateStats(); // restore real attack
        }
    }
    void OnClickInfiniteMoney()
    {
        m_player.AddGoldCoins(999999999);
    }
    void OnClickInfiniteStamina()
    {
        infiniteStamina = !infiniteStamina;

        if (infiniteStamina)
        {
            infiniteStaminaButton.GetComponent<Image>().color = Color.green;
            StartCoroutine(KeepStaminaFull());
        }
        else
        {
            infiniteStaminaButton.GetComponent<Image>().color = Color.white;
            StopCoroutine(KeepStaminaFull());
        }
    }

    IEnumerator KeepStaminaFull()
    {
        while (infiniteStamina)
        {
            m_playerMovement.currentStamina = m_playerMovement.maxStamina;
            yield return null;
        }
    }

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
