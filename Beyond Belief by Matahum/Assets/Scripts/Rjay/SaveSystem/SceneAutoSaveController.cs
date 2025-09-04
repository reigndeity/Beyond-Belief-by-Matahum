using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoSaveController : MonoBehaviour
{
    // No need for a separate slotId anymore — pull directly from GameManager
    string SlotFile => $"Auto_{GameManager.instance.slotId}.es3";

    public void SaveSceneNow()
    {
        ES3AutoSaveMgr.Current.settings.path = SlotFile;

        // Save current scene name
        ES3.Save("Meta.ActiveScene", SceneManager.GetActiveScene().name, SlotFile);

        // Save revealed fog areas (MapManager)
        if (MapManager.instance != null)
        {
            var ids = MapManager.instance.GetRevealedAreaIds();
            ES3.Save("Map.RevealedAreas", ids, SlotFile);
        }

        // Save unlocked teleport statues (by GUID)
        var statues = FindObjectsByType<TeleportInteractable>(FindObjectsSortMode.None);
        var unlockedGuids = new List<string>();
        foreach (var s in statues)
        {
            if (s != null && s.IsUnlocked())
            {
                var g = s.GetGuid();
                if (!string.IsNullOrEmpty(g))
                    unlockedGuids.Add(g);
            }
        }
        ES3.Save("Teleport.Unlocked", unlockedGuids, SlotFile);

        // Save everything configured in ES3 Auto Save window
        ES3AutoSaveMgr.Current.Save();

        Debug.Log($"[AutoSave] Saved to {SlotFile}");
    }

    public void LoadSceneNow() => StartCoroutine(LoadRoutine());

    private IEnumerator LoadRoutine()
    {
        var file = SlotFile;
        ES3AutoSaveMgr.Current.settings.path = file;

        // 1) Ensure we're in the right scene
        if (ES3.KeyExists("Meta.ActiveScene", file))
        {
            var target = ES3.Load<string>("Meta.ActiveScene", file);
            if (!string.IsNullOrEmpty(target) && SceneManager.GetActiveScene().name != target)
            {
                var op = SceneManager.LoadSceneAsync(target);
                while (!op.isDone) yield return null;
            }
        }

        // 2) Load autosave objects
        ES3AutoSaveMgr.Current.Load();

        // 3) Restore revealed fog
        if (ES3.KeyExists("Map.RevealedAreas", file))
        {
            var ids = ES3.Load<List<string>>("Map.RevealedAreas", file);
            if (MapManager.instance != null)
            {
                MapManager.instance.SetRevealedAreas(ids);
                MapManager.instance.SyncRevealVisuals();
            }
        }

        // 4) Restore unlocked teleports
        if (ES3.KeyExists("Teleport.Unlocked", file))
        {
            var unlocked = ES3.Load<List<string>>(file) ?? new List<string>();
            var statues = FindObjectsByType<TeleportInteractable>(FindObjectsSortMode.None);
            foreach (var s in statues)
            {
                if (s == null) continue;
                var g = s.GetGuid();
                if (!string.IsNullOrEmpty(g) && unlocked.Contains(g))
                {
                    s.ForceUnlockSilent(true);
                }
            }
        }
    }

    #region ACCESSIBLE FUNCTIONS
    public async void SaveAll()
    {
        SaveSceneNow();
        await SaveManager.Instance.SaveAsync(GameManager.instance.slotId);
    }
    public async void LoadAll()
    {
        LoadSceneNow();
        await SaveManager.Instance.LoadAsync(GameManager.instance.slotId);
    }
    #endregion
}
