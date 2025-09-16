using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(DialogueStateHolder))]
public class DialogueStateSaver : MonoBehaviour, ISaveable
{
    private DialogueStateHolder holder;

    // Each NPC needs a unique SaveId → use a GUID or unique name
    [SerializeField] private string uniqueId;

    void Awake()
    {
        holder = GetComponent<DialogueStateHolder>();
        if (string.IsNullOrEmpty(uniqueId))
            uniqueId = gameObject.name; // fallback, but better to assign manually in inspector

        SaveManager.Instance.Register(this);
    }

    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => $"DialogueState.{uniqueId}";

    [System.Serializable]
    class DTO { public string currentState; }

    public string CaptureJson()
    {
        var dto = new DTO { currentState = holder.currentState };
        return JsonUtility.ToJson(dto, false);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto == null) return;

        // Restore silently (so you don’t accidentally fire enter/exit twice)
        holder.currentState = dto.currentState;
        holder.TriggerStateEnter(holder.currentState); // ensure events for that state are active
    }
}
