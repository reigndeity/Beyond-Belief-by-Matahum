using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class PersistentGuid : MonoBehaviour
{
    [SerializeField, HideInInspector] private string guid;
    public string Guid => guid;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            GenerateNewGuid();
        }
        else
        {
#if UNITY_EDITOR
            // Check for duplicates in the scene
            var allGuids = UnityEngine.Object.FindObjectsOfType<PersistentGuid>();
            foreach (var g in allGuids)
            {
                if (g != this && g.Guid == guid)
                {
                    // Duplicate detected → regenerate
                    GenerateNewGuid();
                    break;
                }
            }
#endif
        }
    }

    private void GenerateNewGuid()
    {
        guid = System.Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [ContextMenu("Regenerate GUID (Use Carefully)")]
    private void Regenerate()
    {
        GenerateNewGuid();
    }
}