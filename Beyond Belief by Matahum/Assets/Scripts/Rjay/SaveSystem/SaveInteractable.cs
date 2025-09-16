using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveInteractable : Interactable
{
    private Player m_player;

    [Header("Save UI References")]
    public GameObject savePanel;           
    public Button yesButton;               
    public Button noButton;                
    public Button closeButton;
    public TextMeshProUGUI feedbackText;   

    [Header("Events")]
    public UnityEvent onInteract;      // Fired when interacted
    public UnityEvent onSaveStarted;   // Fired when save begins
    public UnityEvent onSaveFinished;  // Fired when save ends
    public UnityEvent onSaveCancelled; // Fired when player presses "No"

    private bool isSaving = false;

    void Awake()
    {
        m_player = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        if (savePanel != null) savePanel.SetActive(false);

        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);

        closeButton.onClick.AddListener(OnClickClose);
    }

    public override void OnInteract()
    {
        if (useInteractCooldown && IsOnCooldown()) return;
        if (isSaving) return;

        base.OnInteract();

        if (savePanel != null) savePanel.SetActive(true);
        if (feedbackText != null) feedbackText.text = "Do you want to save your progress?";
        LockPlayer();
        closeButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        // 🔹 Invoke UnityEvent
        onInteract?.Invoke();
    }

    private void OnYesClicked()
    {
        if (isSaving) return;
        StartCoroutine(SaveRoutine());
    }

    private void OnNoClicked()
    {
        if (savePanel != null) savePanel.SetActive(false);
        PlayerCamera.Instance.SetCursorVisibility(false);
        UnlockPlayer();

        // 🔹 Invoke UnityEvent
        onSaveCancelled?.Invoke();
    }

    private IEnumerator SaveRoutine()
    {
        isSaving = true;
        if (feedbackText != null) feedbackText.text = "Saving...";
        closeButton.interactable = false;
        closeButton.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        onSaveStarted?.Invoke();

        // ✅ Await Task directly instead of bridging
        var task = SaveProgressAsync();
        while (!task.IsCompleted) yield return null;
        if (task.Exception != null) throw task.Exception;

        if (feedbackText != null) feedbackText.text = "Your progress has been saved!";
        closeButton.interactable = true;
        isSaving = false;

        onSaveFinished?.Invoke();
    }


    public void LockPlayer()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);
        PlayerCamera.Instance.HardLockCamera();
        m_player.SetPlayerLocked(true);
    }
    public void UnlockPlayer()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);
        PlayerCamera.Instance.HardUnlockCamera();
        m_player.SetPlayerLocked(false);
    }

    public void OnClickClose()
    {
        savePanel.SetActive(false);
        UnlockPlayer();
    }

    async Task SaveProgressAsync()
    {
        await GameManager.instance.SaveAll();
    }
}

// Helper extension so async Tasks can be awaited in Coroutines
public static class TaskExtensions
{
    public static IEnumerator AsIEnumerator(this System.Threading.Tasks.Task task)
    {
        while (!task.IsCompleted) yield return null;
        if (task.Exception != null) throw task.Exception;
    }
}
