using UnityEngine;
using MTAssets.EasyMinimapSystem;
using System.Collections;

[RequireComponent(typeof(PersistentGuid))]
public class TeleportInteractable : Interactable, ISaveable
{
    [Header("Statue Type")]
    [SerializeField] private bool isSacredStatue = false;

    [Header("Sacred Statue Reveal")]
    [Tooltip("IDs of map areas to reveal on unlock (must match MapManager areas).")]
    [SerializeField] private string[] revealAreaIds;

    [Header("State")]
    [SerializeField] private bool isUnlocked = false;
    private bool wasPreviouslyUnlocked = false;

    [Header("Prompt Icon (world interaction prompt)")]
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
    [SerializeField] private Transform boatObject; // assign your BoatRoot here
    [SerializeField] private float unlockScaleDuration = 1.5f;
    [SerializeField] private float unlockSpinSpeed = 360f; // degrees per second
    [SerializeField] private float idleBobHeight = 0.2f;
    [SerializeField] private float idleBobSpeed = 2f;
    [SerializeField] private float idleRockAngle = 5f;
    [SerializeField] private float idleRockSpeed = 2f;
    [SerializeField] private float blendDuration = 1.0f;

    private Coroutine boatRoutine;
    private bool appliedOnceThisEnable = false;

    private PlayerMinimap playerMinimap;
    private PersistentGuid persistentGuid;

    // ---- LIFECYCLE ----
    private void Awake()
    {
        playerMinimap = FindFirstObjectByType<PlayerMinimap>();
        persistentGuid = GetComponent<PersistentGuid>();

        // NEW: register with SaveManager so each statue persists by GUID in save.json
        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        // NEW: unregister on destroy
        SaveManager.Instance?.Unregister(this);
    }

    private void OnEnable()
    {
        appliedOnceThisEnable = false;
        // ApplyAll will run after save restore
    }

    // ---- INTERACTION ----
    public override void OnInteract()
    {
        if (useInteractCooldown && IsOnCooldown()) return;
        if (useInteractCooldown) TriggerCooldown();

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
        else
            Debug.LogWarning("PlayerMinimap not found in scene.");
    }

    // ---- STATE ----
    public void SetUnlocked(bool value)
    {
        if (isUnlocked == value && appliedOnceThisEnable) return;

        isUnlocked = value;
        ApplyAll();
        StartCoroutine(ApplyIconNextFrame());

        if (boatRoutine != null) StopCoroutine(boatRoutine);

        if (isUnlocked)
        {
            if (!wasPreviouslyUnlocked)
            {
                // First unlock → play animation
                boatRoutine = StartCoroutine(PlayUnlockAnimation());
                wasPreviouslyUnlocked = true;
            }
            else
            {
                // Already unlocked on load → idle loop
                if (boatObject != null) boatRoutine = StartCoroutine(BoatIdleLoop());
            }
        }
        else
        {
            if (boatObject != null) boatObject.localScale = Vector3.zero;
        }
    }

    private void ApplyAll()
    {
        RefreshUI();
        RefreshPromptIcon();
        RefreshMinimapIcon();

        InteractionManager.Instance?.NotifyInteractableChanged(this);
    }

    private void RefreshUI()
    {
        interactName = isUnlocked ? teleportLabel : unlockLabel;
    }

    private void RefreshPromptIcon()
    {
        icon = isUnlocked ? unlockedPromptIcon : lockedPromptIcon;
    }

    private void RefreshMinimapIcon()
    {
        if (minimapItem != null)
        {
            var targetSprite = isUnlocked ? unlockedMinimapSprite : lockedMinimapSprite;
            if (targetSprite != null)
                minimapItem.itemSprite = targetSprite;
        }
    }

    private IEnumerator ApplyIconNextFrame()
    {
        yield return null;
        RefreshMinimapIcon();
        appliedOnceThisEnable = true;
    }

    // ---- SAVE SYSTEM ----
    public string SaveId => persistentGuid != null ? persistentGuid.Guid : null;

    [System.Serializable]
    public class TeleportInteractableSaveData
    {
        public bool isUnlocked;
        public bool wasPreviouslyUnlocked;
    }

    public string CaptureJson()
    {
        var saveData = new TeleportInteractableSaveData
        {
            isUnlocked = this.isUnlocked,
            wasPreviouslyUnlocked = this.wasPreviouslyUnlocked
        };
        return JsonUtility.ToJson(saveData);
    }

    public void RestoreFromJson(string json)
    {
        var saveData = JsonUtility.FromJson<TeleportInteractableSaveData>(json);

        wasPreviouslyUnlocked = saveData.wasPreviouslyUnlocked;

        // Force apply regardless of appliedOnceThisEnable
        appliedOnceThisEnable = false;
        SetUnlocked(saveData.isUnlocked);
    }

    public string GetGuid() => persistentGuid != null ? persistentGuid.Guid : null;
    public bool IsUnlocked() => isUnlocked;
    public void ForceUnlockSilent(bool value) => SetUnlocked(value);

    // ---- SEQUENCES ----
    private IEnumerator UnlockSequence()
    {
        SetUnlocked(true);
        yield return StartCoroutine(PlayUnlockAnimation());

        if (playerMinimap == null)
            playerMinimap = FindFirstObjectByType<PlayerMinimap>();

        if (playerMinimap != null)
            playerMinimap.OpenMapAndCenter();

        MapManager.instance.RevealAreas(revealAreaIds);
    }

    // ---- BOAT ANIMATIONS ----
    private IEnumerator PlayUnlockAnimation()
    {
        if (boatObject == null) yield break;

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

            boatObject.Rotate(Vector3.up, Mathf.Lerp(unlockSpinSpeed, 0f, t) * Time.deltaTime, Space.Self);

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
}