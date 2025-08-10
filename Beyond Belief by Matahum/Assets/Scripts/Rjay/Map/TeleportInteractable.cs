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

    private PlayerMinimap playerMinimap;
    private bool appliedOnceThisEnable = false;
    private PersistentGuid persistentGuid; // NEW

    private void Awake()
    {
        playerMinimap = FindFirstObjectByType<PlayerMinimap>();
        persistentGuid = GetComponent<PersistentGuid>(); // NEW
        ApplyAll();
    }

    private void OnEnable()
    {
        appliedOnceThisEnable = false;
        ApplyAll();
        StartCoroutine(ApplyIconNextFrame()); // EMS sometimes overwrites on Start
    }

    public override void OnInteract()
    {
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
        SetUnlocked(true);

        if (isSacredStatue && MapManager.instance != null)
        {
            if (revealAreaIds != null && revealAreaIds.Length > 0)
                MapManager.instance.RevealAreas(revealAreaIds);
            else
                Debug.LogWarning($"Sacred statue '{name}' has no revealAreaIds set.");
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
                minimapItem.itemSprite = targetSprite; // EMS field in your version
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
    public void ForceUnlockSilent(bool value) => SetUnlocked(value); // for load (no extra reveal)
}
