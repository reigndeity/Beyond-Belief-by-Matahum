using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveInteractable : Interactable
{
    private Player m_player;
    [Header("Save UI References")]
    public GameObject savePanel;           // The confirmation panel
    public Button yesButton;               // "Yes" button
    public Button noButton;                // "No" button
    public Button closeButton;
    public TextMeshProUGUI feedbackText;              // Feedback text (optional, can be in the panel)


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
        // Block if saving already or cooldown active
        if (useInteractCooldown && IsOnCooldown()) return;
        if (isSaving) return;

        base.OnInteract(); // triggers cooldown coroutine

        // Open confirmation panel
        if (savePanel != null) savePanel.SetActive(true);
        if (feedbackText != null) feedbackText.text = "Do you want to save your progress?";
        LockPlayer();
        closeButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

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
    }

    private IEnumerator SaveRoutine()
    {
        isSaving = true;
        if (feedbackText != null) feedbackText.text = "Saving...";
        closeButton.interactable = false;
        closeButton.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // Call GameManager to save
        yield return GameManager.instance.SaveAll().AsIEnumerator();

        if (feedbackText != null) feedbackText.text = "Your progress has been saved!";
        closeButton.interactable = true;
        isSaving = false;
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
