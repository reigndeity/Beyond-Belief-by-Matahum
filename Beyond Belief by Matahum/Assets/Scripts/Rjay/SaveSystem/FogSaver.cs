using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-65)]  // run before GameManager
[DisallowMultipleComponent]
public class FogSaver : MonoBehaviour, ISaveable
{
    void Awake() => SaveManager.Instance.Register(this);
    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Fog.Main";

    [System.Serializable]
    class DTO { public List<string> revealed; }

    public string CaptureJson()
    {
        if (MapManager.instance == null) return null;
        return JsonUtility.ToJson(new DTO {
            revealed = MapManager.instance.GetRevealedAreaIds()
        });
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto?.revealed != null)
        {
            // Delay by one frame so MapManager finishes Awake/Start
            StartCoroutine(RestoreNextFrame(dto.revealed));
        }
    }

    private System.Collections.IEnumerator RestoreNextFrame(List<string> ids)
    {
        yield return null; // wait one frame
        if (MapManager.instance != null)
        {
            MapManager.instance.SetRevealedAreas(ids);
            MapManager.instance.SyncRevealVisuals();
            Debug.Log($"[FogSaver] Restored {ids.Count} revealed areas.");
        }
    }
}
