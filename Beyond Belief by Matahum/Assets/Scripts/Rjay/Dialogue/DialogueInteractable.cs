using UnityEngine;

public class DialogueInteractable : Interactable
{
    [Header("Dialogue")]
    public DialogueSequence dialogueSequence;
    public bool disableAfterPlay = false;

    private DialogueStateHolder m_stateHolder;

    void Awake()
    {
        m_stateHolder = GetComponent<DialogueStateHolder>();
    }

    public override void OnInteract()
    {
        if (DialogueManager.Instance == null || dialogueSequence == null)
            return;

        DialogueManager.Instance.StartDialogue(dialogueSequence, m_stateHolder);

        if (disableAfterPlay)
            gameObject.SetActive(false);
    }
}
