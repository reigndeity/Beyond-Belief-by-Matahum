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
    public bool invertCondition = false;

    public bool isChoiceLine = false;
    public DialogueChoice[] choices;

    public string lineLabel; // NEW: optional label to target this line
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string jumpToLabel; // NEW: string-based label to jump to
    public string requiredCondition;
    public bool invertCondition = false;
}
