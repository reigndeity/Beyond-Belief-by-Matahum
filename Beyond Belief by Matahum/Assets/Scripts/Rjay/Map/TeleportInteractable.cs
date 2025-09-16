using UnityEngine;
using MTAssets.EasyMinimapSystem;
using System.Collections;

[RequireComponent(typeof(PersistentGuid))]
public class TeleportInteractable : Interactable
{
    [Header("Statue Type")]
    [SerializeField] private bool isSacredStatue = false;

    [Header("Sacred Statue Reveal")]
    [Tooltip("IDs of map areas to reveal on unlock (must match MapManager areas).")]
    [SerializeField] private string[] revealAreaIds;

    [Header("State")]
    [SerializeField] private bool isUnlocked = false;

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
    [SerializeField] private float blendDuration = 1.0f; // ✅ new inspector field

    private Coroutine boatRoutine;
    private bool wasPreviouslyUnlocked = false;

    private PlayerMinimap playerMinimap;
    private bool appliedOnceThisEnable = false;
    private PersistentGuid persistentGuid;

    private void Awake()
    {
        playerMinimap = FindFirstObjectByType<PlayerMinimap>();
        persistentGuid = GetComponent<PersistentGuid>();
        ApplyAll();
    }

    private void OnEnable()
    {
        appliedOnceThisEnable = false;
        ApplyAll();
        StartCoroutine(ApplyIconNextFrame());
    }

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
            // Run unlock flow including boat animation + map reveal
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
                // ✅ First unlock → play animation
                boatRoutine = StartCoroutine(PlayUnlockAnimation());
                wasPreviouslyUnlocked = true;
            }
            else
            {
                // ✅ Already unlocked (on load) → idle directly
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

    // --- Save system hooks ---
    public string GetGuid() => persistentGuid != null ? persistentGuid.Guid : null;
    public bool IsUnlocked() => isUnlocked;
    public void ForceUnlockSilent(bool value) => SetUnlocked(value);

    private IEnumerator UnlockSequence()
    {
        // Mark as unlocked (but don’t open map yet)
        SetUnlocked(true);

        // Wait for the boat animation to finish
        yield return StartCoroutine(PlayUnlockAnimation());

        // ✅ Now open the map after animation
        if (playerMinimap == null)
            playerMinimap = FindFirstObjectByType<PlayerMinimap>();

        if (playerMinimap != null)
            playerMinimap.OpenMapAndCenter();

        //yield return new WaitForSeconds(1f);

        MapManager.instance.RevealAreas(revealAreaIds);
    }


    // --- Boat Animations ---
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

        // Blend into idle
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

            // Ease out spin
            boatObject.Rotate(Vector3.up, Mathf.Lerp(unlockSpinSpeed, 0f, t) * Time.deltaTime, Space.Self);

            // Blend into bob + rock
            float bobOffset = Mathf.Lerp(0, Mathf.Sin(Time.time * idleBobSpeed) * idleBobHeight, t);
            float rockAngle = Mathf.Lerp(0, Mathf.Sin(Time.time * idleRockSpeed) * idleRockAngle, t);

            boatObject.localPosition = basePos + Vector3.up * bobOffset;
            boatObject.localRotation = baseRot * Quaternion.Euler(0, 0, rockAngle);

            yield return null;
        }

        // ✅ Start idle loop
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
