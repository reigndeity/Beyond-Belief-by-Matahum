using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueStateEvents
{
    public string stateName;
    public UnityEvent onStateEnter;
    public UnityEvent onStateExit;
}

public class DialogueStateHolder : MonoBehaviour
{
    [Tooltip("The current dialogue state for this NPC.")]
    public string currentState = "Default";

    [Header("Per-State Events")]
    public DialogueStateEvents[] stateEvents;

    private string queuedState = null;
    private string previousState = null;

    public void SetDialogueState(string newState)
    {
        queuedState = newState;
    }

    public void ApplyQueuedState()
    {
        if (!string.IsNullOrEmpty(queuedState))
        {
            previousState = currentState;

            // Exit old state
            TriggerStateExit(previousState);

            // Enter new state
            currentState = queuedState;
            queuedState = null;
            TriggerStateEnter(currentState);
        }
    }

    public bool MatchesState(string test)
    {
        return string.IsNullOrEmpty(test) || test == currentState;
    }

    public void TriggerStateEnter(string state)
    {
        foreach (var e in stateEvents)
        {
            if (e.stateName == state)
            {
                e.onStateEnter?.Invoke();
                break;
            }
        }
    }

    public void TriggerStateExit(string state)
    {
        foreach (var e in stateEvents)
        {
            if (e.stateName == state)
            {
                e.onStateExit?.Invoke();
                break;
            }
        }
    }

    public void ApplyQueuedStateSilently()
    {
        if (!string.IsNullOrEmpty(queuedState))
        {
            previousState = currentState;
            currentState = queuedState;
            queuedState = null;
            // no TriggerStateEnter or Exit
        }
    }
}
