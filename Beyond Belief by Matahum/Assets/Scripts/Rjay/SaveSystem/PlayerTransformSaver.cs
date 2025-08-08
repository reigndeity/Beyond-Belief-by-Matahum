using UnityEngine;

[DisallowMultipleComponent]
public class PlayerTransformSaver : MonoBehaviour, ISaveable
{
    public bool snapToGroundOnLoad = true;
    public float raycastDownDistance = 5f;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        SaveManager.Instance.Register(this);
    }
    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Player.Transform";

    [System.Serializable]
    class DTO { public float[] pos; public float yaw; }

    public string CaptureJson()
    {
        var t = transform;
        var dto = new DTO
        {
            pos = new[] { t.position.x, t.position.y, t.position.z },
            yaw = t.eulerAngles.y
        };
        return JsonUtility.ToJson(dto);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto == null || dto.pos == null || dto.pos.Length < 3) return;

        if (cc) cc.enabled = false;

        transform.SetPositionAndRotation(
            new Vector3(dto.pos[0], dto.pos[1], dto.pos[2]),
            Quaternion.Euler(0f, dto.yaw, 0f)
        );

        if (snapToGroundOnLoad &&
            Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down,
                out var hit, raycastDownDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }

        if (cc) cc.enabled = true;
    }
}
