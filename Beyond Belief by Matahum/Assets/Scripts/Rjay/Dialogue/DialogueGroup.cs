using UnityEngine;

[System.Serializable]
public class DialogueGroup
{
    public string groupName; // e.g., "Act 1", "After Talking to Francis", etc.
    public DialogueLine[] lines;
}
