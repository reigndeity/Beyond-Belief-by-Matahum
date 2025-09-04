using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private SceneAutoSaveController m_sceneAutoSaveController;
    [Header("Save Slot")]
    public string slotId = "Slot_01"; // customize if you want multiple slots

    private string SaveDir => Path.Combine(Application.persistentDataPath, "Saves");
    private string ES3File => $"Auto_{slotId}.es3";
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
    }
    void Start()
    {
        StartCoroutine(LoadPlayer());
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            await SaveAll();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            await LoadAll();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteAll();
        }
    }

    IEnumerator LoadPlayer()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
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

    public async Task LoadAll()
    {
        if (SceneAutoSaveControllerExists())
            FindFirstObjectByType<SceneAutoSaveController>().LoadSceneNow();

        BB_QuestManager.Instance?.LoadQuestData();

        await SaveManager.Instance.LoadAsync(slotId);

        Debug.Log("📂 [GameSaveController] LoadAll complete.");

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

        var es3Path = Path.Combine(Application.persistentDataPath, ES3File);
        if (File.Exists(es3Path))
        {
            File.Delete(es3Path);
            Debug.Log($"🗑️ Deleted ES3 file: {es3Path}");
        }

        BB_QuestManager.Instance?.ClearQuestSaveData();
    }

    private bool SceneAutoSaveControllerExists()
    {
        return FindFirstObjectByType<SceneAutoSaveController>() != null;
    }

}
