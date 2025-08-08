using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public bool IsBusy { get; private set; }

    readonly List<ISaveable> _saveables = new();
    string SavesRoot => Path.Combine(Application.persistentDataPath, "Saves");

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(ISaveable s)   { if (!_saveables.Contains(s)) _saveables.Add(s); }
    public void Unregister(ISaveable s) { _saveables.Remove(s); }

    public async Task SaveAsync(string slot)
    {
        if (IsBusy) return; IsBusy = true;
        try
        {
            var payload = new SavePayload
            {
                activeScene = SceneManager.GetActiveScene().name,
                systems = new List<SystemEntry>()
            };

            foreach (var s in _saveables)
                payload.systems.Add(new SystemEntry { id = s.SaveId, json = s.CaptureJson() });

            var dir = Path.Combine(SavesRoot, slot);
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, "save.json");
            var tmp  = Path.Combine(dir, "save.tmp");
            var bak  = Path.Combine(dir, "save.bak");

            var json = JsonUtility.ToJson(payload, true);
            await File.WriteAllTextAsync(tmp, json);
            if (File.Exists(path)) { File.Copy(path, bak, true); File.Delete(path); }
            File.Move(tmp, path);
        }
        finally { IsBusy = false; }
    }

    public async Task<bool> LoadAsync(string slot)
    {
        if (IsBusy) return false; IsBusy = true;
        try
        {
            var path = Path.Combine(SavesRoot, slot, "save.json");
            if (!File.Exists(path)) return false;

            var json = await File.ReadAllTextAsync(path);
            var payload = JsonUtility.FromJson<SavePayload>(json);

            // scene swap if needed
            var target = payload.activeScene;
            if (!string.IsNullOrEmpty(target) && target != SceneManager.GetActiveScene().name)
            {
                var op = SceneManager.LoadSceneAsync(target);
                while (!op.isDone) await Task.Yield();
                await Task.Yield(); await Task.Yield(); // let savers register
            }

            var map = payload.systems?.ToDictionary(e => e.id, e => e.json) ?? new Dictionary<string,string>();
            foreach (var s in _saveables)
                if (map.TryGetValue(s.SaveId, out var jsonStr))
                    s.RestoreFromJson(jsonStr);

            return true;
        }
        finally { IsBusy = false; }
    }

    [System.Serializable]
    public class SystemEntry { public string id; public string json; }

    [System.Serializable]
    public class SavePayload
    {
        public string activeScene;
        public List<SystemEntry> systems;
    }
}
