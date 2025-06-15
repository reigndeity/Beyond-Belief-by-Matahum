using UnityEngine;

public class PlayerGrassInteraction : MonoBehaviour
{
    public float yOffset = 0.5f;
    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position + Vector3.up * yOffset);
    }
}
