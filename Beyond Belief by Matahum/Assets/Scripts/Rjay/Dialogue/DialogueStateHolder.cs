using UnityEngine;

public class DialogueStateHolder : MonoBehaviour
{
    [Tooltip("The current dialogue state for this NPC.")]
    public string currentState = "Default";

    private string queuedState = null;

    public void SetDialogueState(string newState)
    {
        queuedState = newState;
    }

    public void ApplyQueuedState()
    {
        if (!string.IsNullOrEmpty(queuedState))
        {
            currentState = queuedState;
            queuedState = null;
        }
    }

    public bool MatchesState(string test)
    {
        return string.IsNullOrEmpty(test) || test == currentState;
    }
}
