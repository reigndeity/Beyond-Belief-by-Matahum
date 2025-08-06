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
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string dialogueState = "Default";
}