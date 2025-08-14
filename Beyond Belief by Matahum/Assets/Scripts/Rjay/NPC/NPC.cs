using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class NPC : Interactable
{
    private BlazeAI m_blazeAI;
    private DialogueStateHolder m_stateHolder;
    private Animator m_animator;
    private Transform _player;

    [Header("Dialogue")]
    public DialogueSequence dialogueSequence;
    public bool disableAfterPlay = false;
    public bool blockWhileDialoguePlays = true;

    [Header("Behavior")]
    public Transform faceTransform;
    public float rotationSpeed = 5f;
    private Coroutine faceRoutine;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    [Header("Animation States")]
    public string idle_1;
    public string idle_2, idle_3, smile, closedEyesSmile, angry, wave, curious, curiousToIdle;
    private string currentAnimationState;

    private void Awake()
    {
        m_stateHolder = GetComponent<DialogueStateHolder>();
        m_blazeAI = GetComponent<BlazeAI>();
        m_animator = GetComponent<Animator>();
        faceTransform = transform;

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO) _player = playerGO.transform;
    }

    public override void OnInteract()
    {
        if (!DialogueManager.Instance || !dialogueSequence) return;
        if (blockWhileDialoguePlays && DialogueManager.Instance.IsDialoguePlaying()) return;

        onDialogueStart?.Invoke();

        DialogueManager.Instance.StartDialogue(dialogueSequence, m_stateHolder);
        DialogueManager.Instance.onDialogueEnd.RemoveListener(NotifyDialogueEnded);
        DialogueManager.Instance.onDialogueEnd.AddListener(NotifyDialogueEnded);

        if (disableAfterPlay)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void NotifyDialogueEnded()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveListener(NotifyDialogueEnded);
        onDialogueEnd?.Invoke();
    }

    public void FacePlayer()
    {
        if (!_player) return;
        if (faceRoutine != null) StopCoroutine(faceRoutine);
        faceRoutine = StartCoroutine(FacePlayerRoutine());
    }

    private IEnumerator FacePlayerRoutine()
    {
        Vector3 direction = _player.position - transform.position;
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
        m_animator.CrossFade(newAnimationState, 0.2f);
        currentAnimationState = newAnimationState;
    }

    public string GetAnimationByAlias(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return string.Empty;

        switch (alias.ToLower())
        {
            case "idle_1": return idle_1;
            case "idle_2": return idle_2;
            case "idle_3": return idle_3;
            case "smile": return smile;
            case "closedeyessmile": return closedEyesSmile;
            case "angry": return angry;
            case "wave": return wave;
            case "curious": return curious;
            case "curioustoidle": return curiousToIdle;
            default: return alias; // if not in list, assume it's the animator state name
        }
    }

    public void DialogueStart()
    {
        FacePlayer();

        m_blazeAI.StayIdle();
        m_blazeAI.IgnoreMoveToLocation();

        if (!string.IsNullOrEmpty(idle_1))
        {
            ChangeAnimationState(idle_1);
        }
    }

    public void DialogueEnd()
    {
        StartCoroutine(EndDialogueRoutine());
    }

    private IEnumerator EndDialogueRoutine()
    {
        if (!string.IsNullOrEmpty(idle_1))
        {
            ChangeAnimationState(idle_1);
        }
        yield return new WaitForSeconds(1f);
        m_blazeAI.IgnoreStayIdle();
    }
}
