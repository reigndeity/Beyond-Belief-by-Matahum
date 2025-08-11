using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class PersistentGuid : MonoBehaviour
{
    [SerializeField, HideInInspector] private string guid;
    public string Guid => guid;

    // Generate once and keep forever (unless you force-regenerate)
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString("N");
    }

    [ContextMenu("Regenerate GUID (Use Carefully)")]
    private void Regenerate()
    {
        guid = System.Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
