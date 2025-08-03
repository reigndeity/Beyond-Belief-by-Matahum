using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Optional Voice")]
    public AudioSource voiceSource;

    [Header("Typewriter Settings")]
    public float characterDelay = 0.03f;

    [Header("Input Settings")]
    public KeyCode continueKey = KeyCode.Space;
    public KeyCode fastForwardKey = KeyCode.Mouse0;

    private DialogueSequence currentSequence;
    private List<DialogueLine> flattenedLines = new List<DialogueLine>();
    private int currentIndex;
    private bool isPlaying;
    private bool textFullyRevealed;
    private Coroutine typewriterRoutine;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    [HideInInspector] public bool isDialoguePlaying = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(continueKey) || Input.GetKeyDown(fastForwardKey))
        {
            if (!textFullyRevealed)
            {
                SkipTypewriter();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(DialogueSequence sequence)
    {
        if (sequence == null) return;

        currentSequence = sequence;
        flattenedLines.Clear();
        currentIndex = -1;
        isPlaying = true;
        isDialoguePlaying = true;

        // Flatten groups into one linear list
        foreach (var group in currentSequence.groups)
        {
            flattenedLines.AddRange(group.lines);
        }

        dialoguePanel.SetActive(true);
        onDialogueStart?.Invoke();
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        currentIndex++;

        if (currentIndex >= flattenedLines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = flattenedLines[currentIndex];

        // Check condition
        if (!IsConditionMet(line))
        {
            ShowNextLine();
            return;
        }

        speakerNameText.text = line.speakerName;
        PlayVoiceClip(line.voiceClip);

        if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);
        typewriterRoutine = StartCoroutine(TypeText(line.dialogueText));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        textFullyRevealed = false;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(characterDelay);
        }

        textFullyRevealed = true;
    }

    private void SkipTypewriter()
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
        }

        dialogueText.text = flattenedLines[currentIndex].dialogueText;
        textFullyRevealed = true;
    }

    private void PlayVoiceClip(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    private bool IsConditionMet(DialogueLine line)
    {
        if (string.IsNullOrEmpty(line.requiredCondition)) return true;

        Debug.LogWarning($"[DialogueManager] Condition check skipped: '{line.requiredCondition}' (stubbed as true)");
        return true;
    }

    private void EndDialogue()
    {
        isPlaying = false;
        isDialoguePlaying = false;

        dialoguePanel.SetActive(false);
        currentSequence = null;

        onDialogueEnd?.Invoke();
        FindFirstObjectByType<Player>().suppressInputUntilNextFrame = true;
    }

    public bool IsDialoguePlaying() => isPlaying;
}
