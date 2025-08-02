using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 5)]
    public string dialogueText;

    public AudioClip voiceClip;

    public bool skippable = true;

    public string requiredCondition;  // e.g., "quest_accepted", "" for none
    public bool invertCondition = false;  // if true: show if NOT met
}
