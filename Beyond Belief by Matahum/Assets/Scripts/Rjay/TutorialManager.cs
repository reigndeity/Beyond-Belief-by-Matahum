using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public bool isTutorialDone;

    [Header("Script References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Tutorial Components")]
    public UI_CanvasGroup characterDetailsButton;
    public UI_CanvasGroup inventoryButton;
    public UI_CanvasGroup archiveButton;
    public UI_CanvasGroup questButton;
    public UI_CanvasGroup minimap;
    public UI_CanvasGroup normalSkill;
    public UI_CanvasGroup ultimateSkill;
    public UI_CanvasGroup agimatOne;
    public UI_CanvasGroup agimatTwo;
    public UI_CanvasGroup health;

    [Header("Tupas House")]
    public GameObject temporaryCollider;
    public GameObject tupasHouseStairs;
    public DoorInteractable tupasHouseDoor;
    public GameObject cutsceneTriggerOne;
    public GameObject cutsceneBakalNPC;
    public GameObject lewenriSacredStatue;

    [Header("Player Variables")]
    public bool tutorial_canMovementToggle = true;
    public bool tutorial_canJump = true;
    public bool tutorial_canCameraZoom = true;
    public bool tutorial_canCameraDirection = true;
    public bool tutorial_canAttack;
    public bool tutorial_canSprintAndDash = true;
    public bool tutorial_canNormalSkill = true;
    public bool tutorial_canUltimateSkill = true;
    public bool tutorial_canOpenMap = true;
    public bool tutorial_canToggleMouse = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!isTutorialDone)
        {
            StartTutorial();
        }
        else
        {
            cutsceneBakalNPC.SetActive(false);
        }
    }
    void Update()
    {
        if (!isTutorialDone)
        {
            if (tupasHouseDoor.isOpen)
            {
                cutsceneTriggerOne.SetActive(true);
            }
            else
            {
                cutsceneTriggerOne.SetActive(false);
            }
        }
        else
        {
            return;
        }
    }

    void StartTutorial()
    {
        PlayerCamera.Instance.HardLockCamera();
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q0_InitialTalk");

        // UI
        characterDetailsButton.FadeOut(0);
        inventoryButton.FadeOut(0);
        archiveButton.FadeOut(0);
        questButton.FadeOut(0);
        minimap.FadeOut(0);
        normalSkill.FadeOut(0);
        ultimateSkill.FadeOut(0);
        agimatOne.FadeOut(0);
        agimatTwo.FadeOut(0);   
        health.FadeOut(0);

        // Tupas House
        tupasHouseStairs.SetActive(false);
        temporaryCollider.SetActive(true);
        tupasHouseDoor.interactCooldown = 9999f;
        cutsceneBakalNPC.SetActive(false);

        // Misc
        lewenriSacredStatue.gameObject.layer = LayerMask.NameToLayer("Default");

        // Player Variables
        tutorial_canMovementToggle = false;
        tutorial_canJump = false;
        tutorial_canCameraDirection = false;
        tutorial_canCameraZoom = false;
        tutorial_canAttack = false;
        tutorial_canSprintAndDash = false;
        tutorial_canNormalSkill = false;
        tutorial_canUltimateSkill = false;
        tutorial_canOpenMap = false;
        tutorial_canToggleMouse = false;
        playerMovement.ToggleWalk();  
    }

    public void AllowCameraDirection() => tutorial_canCameraDirection = true;
    public void AllowCameraZoom() => tutorial_canCameraZoom = true;
    public void AllowMovementToggle() => tutorial_canMovementToggle = true;
    public void AllowJump() => tutorial_canJump = true;
    public void AllowAttack() => tutorial_canAttack = true;
    public void AllowDash() => tutorial_canSprintAndDash = true;
    public void AllowNormalSkill() => tutorial_canNormalSkill = true;
    public void AllowUltimateSkill() => tutorial_canUltimateSkill = true;

    public void ShowNormalSkill() => normalSkill.FadeIn(0.5f);
    public void HideNormalSkill() => normalSkill.FadeOut(0.5f);
    public void ShowUltimateSkill() => ultimateSkill.FadeIn(0.5f);
    public void HideUltimateSkill() => ultimateSkill.FadeOut(0.5f);
}
