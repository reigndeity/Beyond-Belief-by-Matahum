using UnityEngine;
using MTAssets.EasyMinimapSystem;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(PersistentGuid))]
public class TeleportInteractable : Interactable, ISaveable
{
    private Player m_player;

    [Header("Statue Type")]
    [SerializeField] private bool isSacredStatue = false;

    [Header("Sacred Statue Reveal")]
    [SerializeField] private string[] revealAreaIds;

    [Header("State")]
    [SerializeField] private bool isUnlocked = false;
    private bool wasPreviouslyUnlocked = false;
    private BB_ArchiveTracker archiveTracker;

    [Header("Prompt Icon")]
    [SerializeField] private Sprite lockedPromptIcon;
    [SerializeField] private Sprite unlockedPromptIcon;

    [Header("Minimap Icon")]
    [SerializeField] private MinimapItem minimapItem;
    [SerializeField] private Sprite lockedMinimapSprite;
    [SerializeField] private Sprite unlockedMinimapSprite;

    [Header("Prompt Text")]
    [SerializeField] private string unlockLabel = "Unlock";
    [SerializeField] private string teleportLabel = "Teleport";

    [Header("Boat Visuals")]
    [SerializeField] private Transform boatObject;
    [SerializeField] private float unlockScaleDuration = 1.5f;
    [SerializeField] private float unlockSpinSpeed = 360f;
    [SerializeField] private float idleBobHeight = 0.2f;
    [SerializeField] private float idleBobSpeed = 2f;
    [SerializeField] private float idleRockAngle = 5f;
    [SerializeField] private float idleRockSpeed = 2f;
    [SerializeField] private float blendDuration = 1.0f;

    private RebultoVFX rebultoVFX;
    private RebultoSFX rebultoSFX;

    [Header("Unlock Events")]
    public UnityEvent OnUnlockedStart;
    public UnityEvent OnUnlockedEnd;

    private Coroutine boatRoutine;
    private PlayerMinimap playerMinimap;
    private PersistentGuid persistentGuid;


    // ---------------------------------------------------------
    // LIFECYCLE
    // ---------------------------------------------------------
    private void Awake()
    {
        m_player = FindFirstObjectByType<Player>();
        playerMinimap = FindFirstObjectByType<PlayerMinimap>();
        persistentGuid = GetComponent<PersistentGuid>();
        archiveTracker = GetComponent<BB_ArchiveTracker>();

        rebultoVFX = GetComponent<RebultoVFX>();
        rebultoSFX = GetComponent<RebultoSFX>();

        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
    }


    // ---------------------------------------------------------
    // INTERACTION
    // ---------------------------------------------------------
    public override void OnInteract()
    {
        if (useInteractCooldown && IsOnCooldown()) return;
        if (useInteractCooldown) TriggerCooldown();

        if (archiveTracker != null) archiveTracker.DiscoveredArchive();

        if (!isUnlocked)
        {
            Unlock();
        }
        else
        {
            OpenFullscreenMap();
        }
    }

    private void Unlock()
    {
        if (isSacredStatue)
        {
            StartCoroutine(UnlockSequence());
        }
        else
        {
            SetUnlocked(true);
        }
    }

    private void OpenFullscreenMap()
    {
        if (playerMinimap == null)
            playerMinimap = FindFirstObjectByType<PlayerMinimap>();

        if (playerMinimap != null)
            playerMinimap.OpenMapAndCenter();
    }


    // ---------------------------------------------------------
    // STATE HANDLING
    // ---------------------------------------------------------
    public void SetUnlocked(bool value)
    {
        bool stateChanged = isUnlocked != value;
        isUnlocked = value;

        ApplyAll();

        if (!isUnlocked)
        {
            if (boatObject != null)
                boatObject.localScale = Vector3.zero;
            return;
        }

        // If this is the very first unlock → play full animation
        if (stateChanged && !wasPreviouslyUnlocked)
        {
            if (boatRoutine != null) StopCoroutine(boatRoutine);
            boatRoutine = StartCoroutine(PlayUnlockAnimation());
            wasPreviouslyUnlocked = true;
        }
        else
        {
            // Scene reload, returning to area, etc.
            ApplyUnlockedVisuals();
        }
    }

    private void ApplyAll()
    {
        RefreshUI();
        RefreshPromptIcon();
        RefreshMinimapIcon();

        InteractionManager.Instance?.NotifyInteractableChanged(this);
    }

    private void RefreshUI() => interactName = isUnlocked ? teleportLabel : unlockLabel;
    private void RefreshPromptIcon() => icon = isUnlocked ? unlockedPromptIcon : lockedPromptIcon;

    private void RefreshMinimapIcon()
    {
        if (minimapItem == null) return;

        minimapItem.itemSprite = isUnlocked
            ? unlockedMinimapSprite
            : lockedMinimapSprite;
    }


    // ---------------------------------------------------------
    // SAVE SYSTEM
    // ---------------------------------------------------------
    public string SaveId => persistentGuid != null ? persistentGuid.Guid : null;

    [System.Serializable]
    public class TeleportInteractableSaveData
    {
        public bool isUnlocked;
        public bool wasPreviouslyUnlocked;
    }

    public string CaptureJson()
    {
        var data = new TeleportInteractableSaveData
        {
            isUnlocked = this.isUnlocked,
            wasPreviouslyUnlocked = this.wasPreviouslyUnlocked
        };
        return JsonUtility.ToJson(data);
    }

    public void RestoreFromJson(string json)
    {
        var data = JsonUtility.FromJson<TeleportInteractableSaveData>(json);

        wasPreviouslyUnlocked = data.wasPreviouslyUnlocked;
        SetUnlocked(data.isUnlocked);
    }

    public string GetGuid() => persistentGuid != null ? persistentGuid.Guid : null;
    public bool IsUnlocked() => isUnlocked;
    public void ForceUnlockSilent(bool value) => SetUnlocked(value);


    // ---------------------------------------------------------
    // VISUAL FIX — THIS IS THE IMPORTANT NEW METHOD
    // ---------------------------------------------------------
    private void ApplyUnlockedVisuals()
    {
        if (rebultoVFX != null) rebultoVFX.AlreadyUnlockedVFX();
        if (rebultoSFX != null) rebultoSFX.PlayUnlockedLoop();

        if (boatRoutine != null)
            StopCoroutine(boatRoutine);

        if (boatObject != null)
        {
            boatObject.localScale = Vector3.one;
            boatRoutine = StartCoroutine(BoatIdleLoop());
        }
    }


    // ---------------------------------------------------------
    // UNLOCK SEQUENCE
    // ---------------------------------------------------------
    private IEnumerator UnlockSequence()
    {
        SetUnlocked(true);

        rebultoVFX.InitialUnlockVFX();
        rebultoSFX.PlayUnlockSFX();

        yield return StartCoroutine(PlayUnlockAnimation());

        if (playerMinimap == null)
            playerMinimap = FindFirstObjectByType<PlayerMinimap>();

        if (playerMinimap) playerMinimap.OpenMapAndCenter();

        MapManager.instance.RevealAreas(revealAreaIds);

        m_player.SetPlayerLocked(false);
        PlayerCamera.Instance.HardUnlockCamera();
        OnUnlockedEnd?.Invoke();
    }


    // ---------------------------------------------------------
    // BOAT ANIMATIONS
    // ---------------------------------------------------------
    private IEnumerator PlayUnlockAnimation()
    {
        if (boatObject == null) yield break;

        m_player.ForceIdleOverride();
        m_player.SetPlayerLocked(true);
        PlayerCamera.Instance.HardLockCamera();
        OnUnlockedStart?.Invoke();

        boatObject.localScale = Vector3.zero;

        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (elapsed < unlockScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unlockScaleDuration);

            boatObject.localScale = Vector3.Lerp(startScale, endScale, t);
            boatObject.Rotate(Vector3.up, unlockSpinSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }

        boatObject.localScale = endScale;
        yield return StartCoroutine(BlendIntoIdle());
    }

    private IEnumerator BlendIntoIdle()
    {
        if (boatObject == null) yield break;

        Vector3 basePos = boatObject.localPosition;
        Quaternion baseRot = boatObject.localRotation;

        float elapsed = 0f;

        while (elapsed < blendDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / blendDuration);

            boatObject.Rotate(Vector3.up, Mathf.Lerp(unlockSpinSpeed, 0f, t) * Time.deltaTime);

            float bobOffset = Mathf.Lerp(0, Mathf.Sin(Time.time * idleBobSpeed) * idleBobHeight, t);
            float rockAngle = Mathf.Lerp(0, Mathf.Sin(Time.time * idleRockSpeed) * idleRockAngle, t);

            boatObject.localPosition = basePos + Vector3.up * bobOffset;
            boatObject.localRotation = baseRot * Quaternion.Euler(0, 0, rockAngle);

            yield return null;
        }

        boatRoutine = StartCoroutine(BoatIdleLoop());
    }

    private IEnumerator BoatIdleLoop()
    {
        if (boatObject == null) yield break;

        Vector3 basePos = boatObject.localPosition;
        Quaternion baseRot = boatObject.localRotation;

        while (true)
        {
            float t = Time.time * idleBobSpeed;
            float bobOffset = Mathf.Sin(t) * idleBobHeight;
            float rockAngle = Mathf.Sin(t * idleRockSpeed) * idleRockAngle;

            boatObject.localPosition = basePos + Vector3.up * bobOffset;
            boatObject.localRotation = baseRot * Quaternion.Euler(0, 0, rockAngle);

            yield return null;
        }
    }

    public void PlayBoatAnimation()
    {
        if (boatObject != null)
            boatObject.localScale = Vector3.one;

        if (boatRoutine != null)
            StopCoroutine(boatRoutine);

        boatRoutine = StartCoroutine(PlayUnlockAnimation());
    }
}
