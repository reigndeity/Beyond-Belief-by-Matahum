using UnityEngine;

[System.Serializable]
public class DialogueGroup
{
    [Tooltip("State where these lines will be shown (e.g. 'Default', 'TalkedToFrancis')")]
    public string dialogueState;

    public DialogueLine[] lines;
}