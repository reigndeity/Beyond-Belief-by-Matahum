using UnityEngine;

public class DialogueInteractable : Interactable
{
    [Header("Dialogue")]
    public DialogueSequence dialogueSequence;
    public bool disableAfterPlay = false;

    private bool dialogueStarted = false;

    public override void OnInteract()
    {
        if (DialogueManager.Instance == null || dialogueSequence == null)
            return;

        var stateHolder = GetComponent<DialogueStateHolder>();
        DialogueManager.Instance.StartDialogue(dialogueSequence, stateHolder);

        if (disableAfterPlay)
            gameObject.SetActive(false);
    }

}
