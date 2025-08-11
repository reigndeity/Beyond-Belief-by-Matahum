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

    // --- Add this helper inside SaveManager ---
    static void UpsertSystemEntry(List<SystemEntry> list, string id, string json)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id) { list[i].json = json; return; }
        list.Add(new SystemEntry { id = id, json = json });
    }

    // --- Add this method: save only selected systems (or all if none passed) ---
    public async Task SaveSystemsAsync(string slot, bool updateScene = false, params string[] ids)
    {
        if (IsBusy) return; IsBusy = true;
        try
        {
            var selected = new System.Collections.Generic.HashSet<string>(ids ?? System.Array.Empty<string>());
            bool saveAll = selected.Count == 0;

            var dir = System.IO.Path.Combine(SavesRoot, slot);
            System.IO.Directory.CreateDirectory(dir);
            var path = System.IO.Path.Combine(dir, "save.json");
            var tmp  = System.IO.Path.Combine(dir, "save.tmp");
            var bak  = System.IO.Path.Combine(dir, "save.bak");

            // Load existing payload if doing a partial update; otherwise start fresh
            SavePayload payload = new SavePayload { activeScene = null, systems = new System.Collections.Generic.List<SystemEntry>() };
            if (!saveAll && System.IO.File.Exists(path))
            {
                var existingJson = await System.IO.File.ReadAllTextAsync(path);
                payload = JsonUtility.FromJson<SavePayload>(existingJson) ?? payload;
            }

            // Update scene name only when saving Transform (or if explicitly asked)
            if (saveAll || updateScene || selected.Contains("Player.Transform"))
                payload.activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // Collect entries to write
            foreach (var s in _saveables)
            {
                if (saveAll || selected.Contains(s.SaveId))
                {
                    var json = s.CaptureJson();
                    if (payload.systems == null) payload.systems = new System.Collections.Generic.List<SystemEntry>();
                    UpsertSystemEntry(payload.systems, s.SaveId, json);
                }
            }

            // Atomic write
            var outJson = JsonUtility.ToJson(payload, true);
            await System.IO.File.WriteAllTextAsync(tmp, outJson);
            if (System.IO.File.Exists(path)) { System.IO.File.Copy(path, bak, true); System.IO.File.Delete(path); }
            System.IO.File.Move(tmp, path);
        }
        finally { IsBusy = false; }
    }
    // --- Add this method inside SaveManager ---
    public async Task LoadSystemsAsync(string slot, bool updateScene = false, params string[] ids)
    {
        if (IsBusy) return; 
        IsBusy = true;
        try
        {
            var path = Path.Combine(SavesRoot, slot, "save.json");
            if (!File.Exists(path)) return;

            var json = await File.ReadAllTextAsync(path);
            var payload = JsonUtility.FromJson<SavePayload>(json);

            // scene swap if needed
            if ((updateScene || ids.Contains("Player.Transform")) &&
                !string.IsNullOrEmpty(payload.activeScene) &&
                payload.activeScene != SceneManager.GetActiveScene().name)
            {
                var op = SceneManager.LoadSceneAsync(payload.activeScene);
                while (!op.isDone) await Task.Yield();
                await Task.Yield(); // let savers register
            }

            var map = payload.systems?.ToDictionary(e => e.id, e => e.json) 
                    ?? new Dictionary<string, string>();

            foreach (var s in _saveables)
            {
                if (ids.Length == 0 || ids.Contains(s.SaveId))
                {
                    if (map.TryGetValue(s.SaveId, out var jsonStr))
                        s.RestoreFromJson(jsonStr);
                }
            }
        }
        finally { IsBusy = false; }
    }



    #region ACCESSIBLE SAVE FUNCTIONS
    public void SavePlayerTransform(string slot)  { _ = SaveSystemsAsync(slot, updateScene: true,  "Player.Transform"); }
    public void SavePlayerStats(string slot)      { _ = SaveSystemsAsync(slot, updateScene: false, "Player.Stats"); }
    public void SavePlayerInventory(string slot)  { _ = SaveSystemsAsync(slot, updateScene: false, "Inventory.Main"); }
    public void SavePlayerEquipment(string slot)  { _ = SaveSystemsAsync(slot, updateScene: false, "Equipment.Main"); }
    #endregion
    #region ACCESSIBLE LOAD FUNCTIONS
    public void LoadPlayerTransform(string slot)  { _ = LoadSystemsAsync(slot, updateScene: true,  "Player.Transform"); }
    public void LoadPlayerStats(string slot)  { _ = LoadSystemsAsync(slot, updateScene: false,  "Player.Stats"); }
    public void LoadPlayerInventory(string slot)  { _ = LoadSystemsAsync(slot, updateScene: false, "Inventory.Main"); }
    public void LoadPlayerEquipment(string slot)  { _ = LoadSystemsAsync(slot, updateScene: false, "Equipment.Main"); }
    #endregion
}
