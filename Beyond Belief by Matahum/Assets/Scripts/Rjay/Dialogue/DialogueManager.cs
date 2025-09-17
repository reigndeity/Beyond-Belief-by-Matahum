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

    [Header("Input Delay Settings")]
    [Tooltip("Time (in seconds) before another skip input is allowed.")]
    public float inputDelaySeconds = 0.25f;

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

    [Header("Line Events")]
    public UnityEvent onLineStart;
    public UnityEvent onLineFinish;

    [HideInInspector] public bool isDialoguePlaying = false;

    private DialogueStateHolder activeStateHolder;

    private bool isEndingDialogue = false;
    private KeyCode lastInputKey = KeyCode.None;
    private float nextInputAllowedTime = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlaying) return;
        if (Time.time < nextInputAllowedTime) return;

        bool spacePressed = Input.GetKeyDown(continueKey);
        bool mousePressed = Input.GetKeyDown(fastForwardKey);

        if (spacePressed && mousePressed) return;

        KeyCode currentKey = KeyCode.None;
        if (spacePressed) currentKey = continueKey;
        if (mousePressed) currentKey = fastForwardKey;
        if (currentKey == KeyCode.None) return;

        lastInputKey = currentKey;
        nextInputAllowedTime = Time.time + inputDelaySeconds;

        if (!waitingForChoice && !inputCooldown && flattenedLines[currentIndex].skippable)
        {
            if (!textFullyRevealed)
            {
                SkipTypewriter();
            }
            else
            {
                // ✅ If last line, end; otherwise show next
                if (currentIndex >= flattenedLines.Count - 1)
                    EndDialogue();
                else
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

            // Animation Handling
            if (activeStateHolder != null)
            {
                NPC npc = activeStateHolder.GetComponent<NPC>();
                if (npc != null)
                {
                    if (!string.IsNullOrEmpty(line.animationName))
                    {
                        string resolvedAnim = npc.GetAnimationByAlias(line.animationName);
                        npc.ChangeAnimationState(resolvedAnim);
                    }
                    else
                    {
                        npc.ChangeAnimationState(npc.GetAnimationByAlias("idle_1"));
                    }
                }
            }

            speakerNameText.text = line.speakerName;
            PlayVoiceClip(line.voiceClip);

            if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);

            onLineStart?.Invoke();
            typewriterRoutine = StartCoroutine(TypeText(line.dialogueText, line));
            return;
        }

        EndDialogue();
    }

    private IEnumerator TypeText(string text, DialogueLine line)
    {
        dialogueText.text = "";
        textFullyRevealed = false;

        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int closingIndex = text.IndexOf('>', i);
                if (closingIndex != -1)
                {
                    string tag = text.Substring(i, closingIndex - i + 1);
                    dialogueText.text += tag;
                    i = closingIndex + 1;
                    continue;
                }
            }

            dialogueText.text += text[i];
            i++;
            yield return new WaitForSeconds(characterDelay);
        }

        textFullyRevealed = true;
        onLineFinish?.Invoke();

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
        onLineFinish?.Invoke();

        // ✅ No auto EndDialogue here anymore — wait for next input if it's the last line
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

        StartDialogue(currentSequence, activeStateHolder);
    }

    private void EndDialogue()
    {
        if (isEndingDialogue) return;
        isEndingDialogue = true;

        if (activeStateHolder != null)
        {
            activeStateHolder.TriggerStateExit(activeStateHolder.currentState);

            string oldState = activeStateHolder.currentState;
            activeStateHolder.ApplyQueuedState();

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

        isEndingDialogue = false;
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

    public void ForceEndDialogue()
    {
        if (!isPlaying) return;
        EndDialogue();
    }
}
