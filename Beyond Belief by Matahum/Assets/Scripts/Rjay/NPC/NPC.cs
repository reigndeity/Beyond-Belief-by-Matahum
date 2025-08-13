using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class NPC : Interactable
{
    [Header("Dialogue")]
    [Tooltip("The dialogue sequence to play for this NPC.")]
    public DialogueSequence dialogueSequence;

    [Tooltip("If true, disables the NPC GameObject immediately after starting dialogue (one-shot).")]
    public bool disableAfterPlay = false;

    private DialogueStateHolder m_stateHolder;

    [Header("Behavior")]
    public Transform faceTransform;
    public float rotationSpeed = 5f;
    private Coroutine faceRoutine;
    private Transform _player; // optional, for facing
    private bool _hasStartedOnce;

    [Tooltip("If true, prevent interaction while a dialogue is already playing.")]
    public bool blockWhileDialoguePlays = true;


    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;   // You can wire this up from DialogueManager callbacks if you expose them.


    [Header("Animation States")]
    private Animator m_animator;
    private string currentAnimationState;
    public string idle_1;
    public string idle_2;
    public string idle_3;
    public string smile;
    public string closedEyesSmile;
    public string angry;
    public string wave;
    public string curious;
    public string curiousToIdle;
    



    void Awake()
    {
        m_stateHolder = GetComponent<DialogueStateHolder>();
        faceTransform = transform;
    }

    void Start()
    {
        // Try to cache player reference for facing (optional).
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO) _player = playerGO.transform;
    }

    void Update()
    {
        HandleGrassInteraction();
    }

    public override void OnInteract()
    {
        if (DialogueManager.Instance == null || dialogueSequence == null)
            return;

        if (blockWhileDialoguePlays && DialogueManager.Instance.IsDialoguePlaying())
            return;

        onDialogueStart?.Invoke();
        DialogueManager.Instance.StartDialogue(dialogueSequence, m_stateHolder);
        DialogueManager.Instance.onDialogueEnd.AddListener(NotifyDialogueEnded);

        if (disableAfterPlay)
        {
            gameObject.SetActive(false);
            return;
        }

        _hasStartedOnce = true;
    }

    public void NotifyDialogueEnded()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveListener(NotifyDialogueEnded);
        onDialogueEnd?.Invoke();
    }

    // Convenience API if you need to swap sequences at runtime
    public void SetDialogue(DialogueSequence seq) => dialogueSequence = seq;
    public void SetDisableAfterPlay(bool value) => disableAfterPlay = value;

    public void FacePlayer()
    {
        if (faceRoutine != null) StopCoroutine(faceRoutine);
        faceRoutine = StartCoroutine(FacePlayerRoutine());
    }

    IEnumerator FacePlayerRoutine()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target == null) yield break;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        if (currentAnimationState == newAnimationState) return;
        m_animator.CrossFade(newAnimationState, 0.2f);
        currentAnimationState = newAnimationState;
    }

    void HandleGrassInteraction() => Shader.SetGlobalVector("_Player", transform.position + Vector3.up * 0.5f);
}
