using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private SceneAutoSaveController m_sceneAutoSaveController;
    [Header("Save Slot")]
    public string slotId = "Slot_01"; // customize if you want multiple slots

    private string SaveDir => Path.Combine(Application.persistentDataPath, "Saves");
    private string ES3File => $"Auto_{slotId}.es3";

    [Header("A2_Q5_WhatHappened")]
    public Transform forcedTeleportTarget;  // set this externally
    public bool hasForcedTeleport = false;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        m_sceneAutoSaveController = FindFirstObjectByType<SceneAutoSaveController>();

        Time.timeScale = 1;
    }
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "OpenWorldScene")
        {
            StartCoroutine(LoadPlayer());
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator LoadPlayer()
    {
        var loadTask = LoadAll();
        yield return new WaitUntil(() => loadTask.IsCompleted);
        yield return new WaitForSeconds(1f);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
    }


    public async Task SaveAll()
    {
        // 1) Save scene data with ES3
        if (SceneAutoSaveControllerExists())
            FindFirstObjectByType<SceneAutoSaveController>().SaveSceneNow();

        // 2) Save quest data
        BB_QuestManager.Instance?.SaveQuestData();

        // 3) Save all ISaveables via SaveManager
        await SaveManager.Instance.SaveAsync(slotId);

        Debug.Log("💾 [GameSaveController] SaveAll complete.");
    }

    public async Task SaveAllExceptPlayerPosition()
    {
        if (SceneAutoSaveControllerExists())
            FindFirstObjectByType<SceneAutoSaveController>().SaveSceneNow();

        BB_QuestManager.Instance?.SaveQuestData();

        // Save everything except Player.Transform (player position)
        await SaveManager.Instance.SaveSystemsExceptAsync(slotId, updateScene: false, "Player.Transform");

        Debug.Log("💾 [GameSaveController] SaveAllExceptPlayerPosition complete.");
    }

    public async Task LoadAll()
    {
        if (SceneAutoSaveControllerExists())
            FindFirstObjectByType<SceneAutoSaveController>().LoadSceneNow();

        BB_QuestManager.Instance?.LoadQuestData();

        await SaveManager.Instance.LoadAsync(slotId);

        Debug.Log("📂 [GameSaveController] LoadAll complete.");

        TutorialManager.instance?.TutorialCheck();

        // ✅ Apply forced teleport if requested
        if (hasForcedTeleport && forcedTeleportTarget != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.SetPositionAndRotation(
                    forcedTeleportTarget.position,
                    forcedTeleportTarget.rotation
                );

                if (cc != null) cc.enabled = true;
            }

            Debug.Log($"✅ Forced teleport applied to {forcedTeleportTarget.name}");
            hasForcedTeleport = false;  // reset after use
        }
    }


    public async Task LoadPlayerCoreData()
    {
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Player.Stats");
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Inventory.Main");
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Equipment.Main");

        BB_QuestManager.Instance?.LoadQuestData();

        Debug.Log("📂 [GameSaveController] Loaded Player Stats + Inventory + Equipment + Quests.");

        TutorialManager.instance?.TutorialCheck();
    }
    public async Task SavePlayerCoreData()
    {
        await SaveManager.Instance.SaveSystemsAsync(slotId, updateScene: false, "Player.Stats");
        await SaveManager.Instance.SaveSystemsAsync(slotId, updateScene: false, "Inventory.Main");
        await SaveManager.Instance.SaveSystemsAsync(slotId, updateScene: false, "Equipment.Main");
        await SaveManager.Instance.SaveSystemsAsync(slotId, updateScene: false, "ChestManager");
        await SaveManager.Instance.SaveSystemsAsync(slotId, updateScene: false, "Fog.Main");

        // Save quests as well
        BB_QuestManager.Instance?.SaveQuestData();

        Debug.Log("💾 [GameSaveController] Saved Player Stats + Inventory + Equipment + Quests.");
        
    }

    public async Task LoadPlayerNoQuest()
    {
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Player.Stats");
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Inventory.Main");
        await SaveManager.Instance.LoadSystemsAsync(slotId, updateScene: false, "Equipment.Main");
        TutorialManager.instance?.TutorialCheck();
    }



    public void DeleteAll()
    {
        var dir = Path.Combine(SaveDir, slotId);
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
            Debug.Log($"🗑️ Deleted SaveManager folder: {dir}");
        }

        // Delete only the current slot
        var es3Path = Path.Combine(Application.persistentDataPath, $"Auto_{slotId}.es3");
        if (File.Exists(es3Path))
        {
            File.Delete(es3Path);
            Debug.Log($"🗑️ Deleted ES3 file: {es3Path}");
        }

        // Cleanup ALL Auto_*.es3 files (old or mismatched)
        foreach (var file in Directory.GetFiles(Application.persistentDataPath, "Auto_*.es3"))
        {
            File.Delete(file);
            Debug.Log($"🗑️ Force deleted leftover ES3 file: {file}");
        }

        BB_QuestManager.Instance?.ClearQuestSaveData();

        PlayerPrefs.DeleteAll();
    }

    private bool SceneAutoSaveControllerExists()
    {
        return FindFirstObjectByType<SceneAutoSaveController>() != null;
    }

    public async void SaveGame()
    {
        await SaveAll();
    }
    
    public async Task SaveOnlyES3()
    {
        if (SceneAutoSaveControllerExists())
        {
            FindFirstObjectByType<SceneAutoSaveController>().SaveSceneNow();
            Debug.Log("💾 [GameSaveController] Saved only ES3 scene data.");
        }
        else
        {
            Debug.LogWarning("⚠️ No SceneAutoSaveController found — ES3 save skipped.");
        }

        await Task.Yield(); // just to keep async signature consistent
    }
}
