using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Debug")]
    public string debugCurrentDialogueState;

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

    [Header("Choice Settings")]
    public GameObject choicePanel;
    public Button choiceButtonPrefab;
    public Transform choiceButtonContainer;

    private DialogueSequence currentSequence;
    private List<DialogueLine> flattenedLines = new();
    private int currentIndex;
    private Coroutine typewriterRoutine;
    private bool isPlaying;
    private bool textFullyRevealed;
    private bool waitingForChoice = false;
    private bool inputCooldown = false;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    [HideInInspector] public bool isDialoguePlaying = false;

    private DialogueStateHolder activeStateHolder;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlaying) return;

        if ((Input.GetKeyDown(continueKey) || Input.GetKeyDown(fastForwardKey)) && !waitingForChoice && !inputCooldown && flattenedLines[currentIndex].skippable)
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

    public void StartDialogue(DialogueSequence sequence, DialogueStateHolder stateHolder = null)
    {
        if (sequence == null) return;

        currentSequence = sequence;
        flattenedLines.Clear();
        currentIndex = -1;
        isPlaying = true;
        isDialoguePlaying = true;

        activeStateHolder = stateHolder;

        if (activeStateHolder != null)
        {
            debugCurrentDialogueState = activeStateHolder.currentState;

            // ✅ Trigger enter event for the current state so animations or events play
            activeStateHolder.TriggerStateEnter(activeStateHolder.currentState);
        }

        foreach (var group in currentSequence.groups)
        {
            if (group.dialogueState == (activeStateHolder != null ? activeStateHolder.currentState : "Default") 
                || string.IsNullOrEmpty(group.dialogueState))
            {
                flattenedLines.AddRange(group.lines);
            }
        }

        dialoguePanel.SetActive(true);
        onDialogueStart?.Invoke();
        ShowNextLine();
    }



    private void ShowNextLine()
    {
        choicePanel.SetActive(false);
        foreach (Transform child in choiceButtonContainer)
            Destroy(child.gameObject);

        while (currentIndex + 1 < flattenedLines.Count)
        {
            currentIndex++;
            DialogueLine line = flattenedLines[currentIndex];

            if (true)
            {
                speakerNameText.text = line.speakerName;
                PlayVoiceClip(line.voiceClip);

                if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);
                typewriterRoutine = StartCoroutine(TypeText(line.dialogueText, line));
                return;
            }
        }

        EndDialogue();  // ✅ Only ends if no valid lines remain
    }

    private IEnumerator TypeText(string text, DialogueLine line)
    {
        dialogueText.text = "";
        textFullyRevealed = false;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(characterDelay);
        }

        textFullyRevealed = true;

        if (line.isChoiceLine && line.choices != null && line.choices.Length > 0)
        {
            DisplayChoices(line.choices);
        }
    }

    private void SkipTypewriter()
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
        }

        dialogueText.text = flattenedLines[currentIndex].dialogueText;
        textFullyRevealed = true;
        StartCoroutine(InputCooldownRoutine());
    }

    private IEnumerator InputCooldownRoutine()
    {
        inputCooldown = true;
        yield return new WaitForSeconds(0.1f);
        inputCooldown = false;
    }

    private void PlayVoiceClip(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    private void DisplayChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);
        waitingForChoice = true;

        foreach (DialogueChoice choice in choices)
        {
            Button choiceBtn = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            choiceBtn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            choiceBtn.onClick.AddListener(() =>
            {
                waitingForChoice = false;
                SelectDialogueState(choice.dialogueState);
            });
        }
    }

    private void SelectDialogueState(string newState)
    {
        SetDialogueState(newState);

        if (activeStateHolder != null)
        {
            activeStateHolder.ApplyQueuedState();
            debugCurrentDialogueState = activeStateHolder.currentState;
        }

        StartDialogue(currentSequence, activeStateHolder);  // ✅ No EndDialogue here
    }

    private void EndDialogue()
    {
        if (activeStateHolder != null)
        {
            // 🔹 Exit event for the old state
            activeStateHolder.TriggerStateExit(activeStateHolder.currentState);

            // 🔹 Apply the queued state change now
            string oldState = activeStateHolder.currentState;
            activeStateHolder.ApplyQueuedState();

            // 🔹 Enter event for the new state (if changed)
            if (activeStateHolder.currentState != oldState)
            {
                activeStateHolder.TriggerStateEnter(activeStateHolder.currentState);
            }

            debugCurrentDialogueState = activeStateHolder.currentState;
        }

        isPlaying = false;
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        currentSequence = null;

        onDialogueEnd?.Invoke();
        FindFirstObjectByType<Player>().suppressInputUntilNextFrame = true;
    }


    public void SetDialogueState(string newState)
    {
        if (activeStateHolder != null)
        {
            activeStateHolder.SetDialogueState(newState);
            debugCurrentDialogueState = newState;
        }
    }

    public string GetCurrentDialogueState()
    {
        return activeStateHolder != null ? activeStateHolder.currentState : "Default";
    }

    public bool IsDialoguePlaying() => isPlaying;
}