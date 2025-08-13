using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class NPC : Interactable
{
    private BlazeAI m_blazeAI;

    [Header("Dialogue")]
    public DialogueSequence dialogueSequence;
    public bool disableAfterPlay = false;

    private DialogueStateHolder m_stateHolder;

    [Header("Behavior")]
    public Transform faceTransform;
    public float rotationSpeed = 5f;
    private Coroutine faceRoutine;
    private Transform _player;
    private bool _hasStartedOnce;

    public bool blockWhileDialoguePlays = true;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    [Header("Animation States")]
    private Animator m_animator;
    private string currentAnimationState;
    public string idle_1, idle_2, idle_3, smile, closedEyesSmile, angry, wave, curious, curiousToIdle;

    void Awake()
    {
        m_stateHolder = GetComponent<DialogueStateHolder>();
        m_animator = GetComponent<Animator>();
        m_blazeAI = GetComponent<BlazeAI>();
        faceTransform = transform;
    }

    void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO) _player = playerGO.transform;
    }

    public override void OnInteract()
    {
        if (DialogueManager.Instance == null || dialogueSequence == null)
            return;

        if (blockWhileDialoguePlays && DialogueManager.Instance.IsDialoguePlaying())
            return;

        onDialogueStart?.Invoke();
        DialogueManager.Instance.StartDialogue(dialogueSequence, m_stateHolder);

        // Make sure we only listen once
        DialogueManager.Instance.onDialogueEnd.RemoveListener(NotifyDialogueEnded);
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
        StartCoroutine(ForceAnimationNextFrame(newAnimationState));
    }

    private IEnumerator ForceAnimationNextFrame(string anim)
    {
        yield return null; // wait 1 frame so BlazeAI finishes
        Debug.Log("Changed animation to: " + anim);
        if (currentAnimationState == anim) yield break;
        m_animator.CrossFade(anim, 0.2f);
        currentAnimationState = anim;
    }

    public void DialogueStart()
    {
        FacePlayer();
        m_blazeAI.StayIdle();
        m_blazeAI.IgnoreMoveToLocation();
        m_blazeAI.CloseLastBehaviour();
       
    }

    public void DialogueEnd()
    {
        m_blazeAI.IgnoreStayIdle();
        m_blazeAI.ChangeState("normal");
    }
}
