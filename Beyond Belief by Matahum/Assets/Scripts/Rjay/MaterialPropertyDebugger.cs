using UnityEngine;

public class MaterialPropertyDebugger : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetRenderer;

    private void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer assigned!");
            return;
        }

        foreach (var mat in targetRenderer.sharedMaterials)
        {
            if (mat == null) continue;

            var shader = mat.shader;
            Debug.Log($"--- Material: {mat.name} | Shader: {shader.name} ---");

            int count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                string propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);
                Debug.Log($"Property: {propName} | Type: {propType}");
            }
        }
    }
}
