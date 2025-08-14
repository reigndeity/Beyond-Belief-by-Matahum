using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 5)]
    public string dialogueText;

    public AudioClip voiceClip;

    public bool skippable = true;

    public bool isChoiceLine = false;
    public DialogueChoice[] choices;

    [Header("Optional Animation")]
    public string animationName; // Leave blank for default animation
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string dialogueState = "Default";
}