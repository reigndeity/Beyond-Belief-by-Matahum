using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    [Header("Raycasting & Prompt Setup")]
    public LayerMask interactableLayer;
    public float detectRange = 3f;
    public Transform rayOrigin;
    public Transform promptParent;
    public GameObject promptPrefab;
    [Header("Prompt Behavior")]
    [SerializeField] private CanvasGroup promptCanvasGroup; // assign the root of your prompt UI
    [SerializeField] private float promptFadeDuration = 0.15f;
    [SerializeField] private float promptReenableDelay = 2f; // your requested delay
    private Coroutine promptFadeCo;

    [Header("Manual Scrolling Settings")]
    [SerializeField] private RectTransform promptContainer;  // The thing we move up/down
    [SerializeField] private float itemHeight = 80f;         // Height of each prompt (incl. spacing)
    [SerializeField] private int visibleCount = 3;           // Max visible prompts at a time

    private List<Interactable> interactablesInView = new();
    private List<Interactable> lastInteractables = new();
    private int currentSelection = 0;

    public static bool IsUsingScrollInput = false;

    void Awake() => Instance = this;

    void Update()
    {
        DetectAndUpdateInteractables();
    }

    void DetectAndUpdateInteractables()
    {
        List<Interactable> detected = new();
        Collider[] hits = Physics.OverlapSphere(rayOrigin.position, detectRange, interactableLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Interactable interactable))
                detected.Add(interactable);
        }

        // Check if the list changed
        bool listChanged = detected.Count != lastInteractables.Count;
        if (!listChanged)
        {
            for (int i = 0; i < detected.Count; i++)
            {
                if (detected[i] != lastInteractables[i])
                {
                    listChanged = true;
                    break;
                }
            }
        }

        if (listChanged)
        {
            lastInteractables = new List<Interactable>(detected);
            interactablesInView = detected;

            // Try to preserve selection
            Interactable previous = (currentSelection < interactablesInView.Count) ? interactablesInView[currentSelection] : null;
            int index = interactablesInView.IndexOf(previous);
            currentSelection = (index >= 0) ? index : 0;

            UpdatePromptUI();
            SnapToVisibleRange();
        }

        HandleScrollInput();
        HandleInteraction();
    }

    void HandleScrollInput()
    {
        IsUsingScrollInput = false;

        if (interactablesInView.Count <= 1) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0f)
        {
            IsUsingScrollInput = true;

            if (scroll > 0f)
                currentSelection = Mathf.Max(0, currentSelection - 1);
            else
                currentSelection = Mathf.Min(interactablesInView.Count - 1, currentSelection + 1);

            RefreshPromptHighlights();
            SnapToVisibleRange();
        }
    }

    void HandleInteraction()
    {
        if (interactablesInView.Count == 0) return;
        
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying()) return;

        var selected = interactablesInView[currentSelection];
        if (Input.GetKeyDown(selected.interactKey))
            selected.OnInteract();
    }

    void UpdatePromptUI()
    {
        foreach (Transform child in promptParent)
            Destroy(child.gameObject);

        for (int i = 0; i < interactablesInView.Count; i++)
        {
            var prompt = Instantiate(promptPrefab, promptParent);
            prompt.GetComponent<InteractionPromptUI>().Setup(interactablesInView[i], i == currentSelection);
        }
    }

    void RefreshPromptHighlights()
    {
        for (int i = 0; i < promptParent.childCount; i++)
        {
            var prompt = promptParent.GetChild(i).GetComponent<InteractionPromptUI>();
            prompt.SetHighlight(i == currentSelection);
        }
    }

    void SnapToVisibleRange()
    {
        if (interactablesInView.Count == 0 || promptContainer == null) return;

        int topIndex;

        if (currentSelection <= 1)
        {
            topIndex = 0;
        }
        else if (currentSelection >= interactablesInView.Count - 1)
        {
            topIndex = interactablesInView.Count - visibleCount;
        }
        else
        {
            topIndex = currentSelection - 1;
        }

        topIndex = Mathf.Clamp(topIndex, 0, Mathf.Max(0, interactablesInView.Count - visibleCount));
        float targetY = topIndex * itemHeight;

        promptContainer.anchoredPosition = new Vector2(0f, targetY);
    }

    public bool HasInteractables() => interactablesInView.Count > 0;

    
}
