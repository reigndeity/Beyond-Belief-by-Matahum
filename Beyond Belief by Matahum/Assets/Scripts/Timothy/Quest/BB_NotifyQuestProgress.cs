using UnityEngine;

public class BB_NotifyQuestProgress : MonoBehaviour
{
    public string targetID; // Must match a mission's targetID in the quest

    [Header("When to notify")]
    public bool notifyOnDestroy = true;
    public bool notifyOnCollect = false; // For items

    private void OnDestroy()
    {
        if (notifyOnDestroy)
        {
            NotifyQuestProgress();
        }
    }

    // Call this manually if collected instead of destroyed
    public void OnCollected()
    {
        if (notifyOnCollect)
        {
            NotifyQuestProgress();
        }
    }

    public void OnInteract()
    {
        NotifyQuestProgress();
    }

    private void NotifyQuestProgress()
    {
        if (!string.IsNullOrEmpty(targetID))
        {
            BB_QuestManager.Instance?.UpdateMissionProgress(targetID, 1);
        }
    }
}
