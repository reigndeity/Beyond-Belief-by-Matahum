using UnityEngine;

public class BB_NotifyQuestProgress : MonoBehaviour
{
    public string targetID; // Must match a mission's targetID in the quest

    [Header("When to notify")]
    public bool notifyOnDestroy = false;
    public bool notifyOnCollect = false; // For items
    public bool notifyOnTrigger = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            if (notifyOnTrigger)
                NotifyQuestProgress();
    }

    private void NotifyQuestProgress()
    {
        if (!string.IsNullOrEmpty(targetID))
        {
            if(BB_QuestManager.Instance != null)
                BB_QuestManager.Instance?.UpdateMissionProgress(targetID, 1);
        }
    }
}
