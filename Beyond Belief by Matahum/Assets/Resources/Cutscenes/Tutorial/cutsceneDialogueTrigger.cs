using UnityEngine;
using UnityEngine.Events;

public class cutsceneDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSequence dialogueSequence;
    public DialogueStateHolder dialogueStateHolder;

    /// <summary>
    /// Plays dialogue during a cutscene and resumes Timeline when done.
    /// </summary>
    public void Play()
    {
        if (CutsceneManager.Instance == null || DialogueManager.Instance == null) return;

        if (dialogueSequence != null)
        {
            UnityAction resumeAction = null;
            resumeAction = () =>
            {
                DialogueManager.Instance.onDialogueEnd.RemoveListener(resumeAction);
                CutsceneManager.Instance.ResumeTimeline();
            };

            DialogueManager.Instance.onDialogueEnd.AddListener(resumeAction);
            DialogueManager.Instance.StartDialogue(dialogueSequence, dialogueStateHolder);
        }
    }
}
