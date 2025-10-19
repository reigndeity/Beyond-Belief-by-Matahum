using UnityEngine;

[DefaultExecutionOrder(-60)]
[DisallowMultipleComponent]
[RequireComponent(typeof(TutorialManager))]
public class TutorialSaver : MonoBehaviour, ISaveable
{
    private TutorialManager tutorial;

    void Awake()
    {
        tutorial = GetComponent<TutorialManager>();
        Debug.Log("TutorialSaver Awake, registering with SaveManager");
        SaveManager.Instance.Register(this);
    }

    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Tutorial.Main";

    [System.Serializable]
    class DTO
    {
        public bool isTutorialDone;

        public bool tutorial_canMovementToggle;
        public bool tutorial_canJump;
        public bool tutorial_canCameraZoom;
        public bool tutorial_canCameraDirection;
        public bool tutorial_canAttack;
        public bool tutorial_canSprintAndDash;
        public bool tutorial_canNormalSkill;
        public bool tutorial_canUltimateSkill;
        public bool tutorial_canOpenMap;
        public bool tutorial_canToggleMouse;
        public bool tutorial_canArchives;

        public bool tutorial_isFirstStatueInteract;
        public bool tutorial_isGateOpen;
        public bool tutorial_isFirstSaveStatueInteract;
        public bool canHotKeyInventory;
        public bool canHotKeyJournal;
        public bool canHotKeyCharacterDetails;
    }

    public string CaptureJson()
    {
        Debug.Log("TutorialSaver CaptureJson");
        var dto = new DTO
        {
            isTutorialDone = tutorial.isTutorialDone,

            tutorial_canMovementToggle = tutorial.tutorial_canMovementToggle,
            tutorial_canJump = tutorial.tutorial_canJump,
            tutorial_canCameraZoom = tutorial.tutorial_canCameraZoom,
            tutorial_canCameraDirection = tutorial.tutorial_canCameraDirection,
            tutorial_canAttack = tutorial.tutorial_canAttack,
            tutorial_canSprintAndDash = tutorial.tutorial_canSprintAndDash,
            tutorial_canNormalSkill = tutorial.tutorial_canNormalSkill,
            tutorial_canUltimateSkill = tutorial.tutorial_canUltimateSkill,
            tutorial_canOpenMap = tutorial.tutorial_canOpenMap,
            tutorial_canToggleMouse = tutorial.tutorial_canToggleMouse,
            tutorial_canArchives = tutorial.tutorial_canArchives,

            tutorial_isFirstStatueInteract = tutorial.tutorial_isFirstStatueInteract,
            tutorial_isGateOpen = tutorial.tutorial_isGateOpen,
            tutorial_isFirstSaveStatueInteract = tutorial.tutorial_isFirstSaveStatueInteract,

            canHotKeyInventory = tutorial.canHotKeyInventory,
            canHotKeyJournal = tutorial.canHotKeyJournal,
            canHotKeyCharacterDetails = tutorial.canHotKeyCharacterDetails
        };
        return JsonUtility.ToJson(dto, false);
    }

    public void RestoreFromJson(string json)
    {
        Debug.Log("TutorialSaver RestoreFromJson");
        if (string.IsNullOrEmpty(json)) return;
        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto == null) return;

        tutorial.isTutorialDone = dto.isTutorialDone;

        tutorial.tutorial_canMovementToggle = dto.tutorial_canMovementToggle;
        tutorial.tutorial_canJump = dto.tutorial_canJump;
        tutorial.tutorial_canCameraZoom = dto.tutorial_canCameraZoom;
        tutorial.tutorial_canCameraDirection = dto.tutorial_canCameraDirection;
        tutorial.tutorial_canAttack = dto.tutorial_canAttack;
        tutorial.tutorial_canSprintAndDash = dto.tutorial_canSprintAndDash;
        tutorial.tutorial_canNormalSkill = dto.tutorial_canNormalSkill;
        tutorial.tutorial_canUltimateSkill = dto.tutorial_canUltimateSkill;
        tutorial.tutorial_canOpenMap = dto.tutorial_canOpenMap;
        tutorial.tutorial_canToggleMouse = dto.tutorial_canToggleMouse;
        tutorial.tutorial_canArchives = dto.tutorial_canArchives;

        tutorial.tutorial_isFirstStatueInteract = dto.tutorial_isFirstStatueInteract;
        tutorial.tutorial_isGateOpen = dto.tutorial_isGateOpen;
        tutorial.tutorial_isFirstSaveStatueInteract = dto.tutorial_isFirstSaveStatueInteract;

        tutorial.canHotKeyInventory = dto.canHotKeyInventory;
        tutorial.canHotKeyJournal = dto.canHotKeyJournal;
        tutorial.canHotKeyCharacterDetails = dto.canHotKeyCharacterDetails;

        // After restoring, re-apply the tutorial state
        tutorial.TutorialCheck();
    }
}
