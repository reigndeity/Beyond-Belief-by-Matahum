using UnityEngine;
using BlazeAISpace;
using UnityEngine.SceneManagement;
using TMPro;

public class ArenaManagerScript : MonoBehaviour
{
    public GameObject UI_TransitionController; // improvent of life

    [Header("Arena Stats")]
    public float MinimumInterval = 0.5f; //change to private later
    public float MaxInterval = 3f; //change to private later
    public int CurrentWaveCount = 0;

    [Header("Scene")]
    public string SceneToLoad;

    [Header("Canvas GameObjects")]
    public GameObject GameOverCanvas;
    public GameObject GameFinishedCanvas;

    [Header("Wave Holders")]
    public GameObject Wave1Holder;
    public GameObject Wave2Holder;
    public GameObject Wave3Holder;

    [Header("Wave 2")]
    public GameObject[] wave2Enemies;

    [Header("Wave 3")]
    public Nuno _nuno;
    public Nuno_AttackManager _nuno_AttackManager;

    [Header("Game Data")]
    public string PlayerID;
    public int GameMode; //0 = Classic 1 = Adaptive
    public float TotalRunTime = 0;
    public float Wave1Time = 0;
    public float Wave2Time = 0;
    public float Wave3Time = 0;
    public float Wave1DamageTaken;
    public float Wave2DamageTaken;
    public float Wave1DamageDealt;
    public float Wave2DamageDealt;
    public float TotalDamageDealt = 0;
    public float TotalDamageTaken = 0;
    public int TotalDeathCount = 0;


    public GameTimer gameTimer;

    //[Header("UI")]

    public void Awake()
    {
        UI_TransitionController.SetActive(true);

        Wave2DifficultyAdjust();
        CurrentWaveCount++;
        WaveCheck();
        Debug.Log("Death Count = " + PlayerPrefs.GetFloat("Total Death Count"));
    }

    public void Update()
    {
        RemainingEnemyCheck();
    }

    public void WaveCheck()
    {
        if (CurrentWaveCount == 1)
        {
            Wave1Holder.SetActive(true);
        } else if (CurrentWaveCount == 2)
        {
            Wave2Holder.SetActive(true);
        }
        else if (CurrentWaveCount == 3)
        {
            Wave3Holder.SetActive(true);
        }
    }

    public void RemainingEnemyCheck()
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        Debug.Log("Enemy count: " + enemies.Length);

        if (enemies.Length == 0)
        {
            Debug.Log("No enemies left.");

            if (CurrentWaveCount == 1)
            {
                Wave1Time = gameTimer.runTime;
                Wave1DamageTaken = FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageTaken;
                Wave1DamageDealt = FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageDealt;
            } else if (CurrentWaveCount == 2)
            {
                Wave2Time = gameTimer.runTime;
                Wave2DamageTaken = FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageTaken;
                Wave2DamageDealt = FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageDealt;
            }
            gameTimer.runTime = 0;
            CurrentWaveCount++;
            TotalDamageDealt += FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageDealt;
            TotalDamageTaken += FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageTaken;
            FindFirstObjectByType<DamageTakenGivenCompiler>().ResetValues();
            WaveCheck();
        }
    }

    public void CurrentWave()
    {
        
    }

    public void OnPlayerDeath()
    {
        GameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        UnlockMouse();
    }

    public void UnlockMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GameFinished()
    {
        Debug.Log("Game Done Show Player Stats");
        FindFirstObjectByType<Player>().m_uiGame.HideUI(); //Hide Player UI


        TotalDamageDealt += FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageDealt;
        TotalDamageTaken += FindFirstObjectByType<DamageTakenGivenCompiler>().CurrentDamageTaken;

        Wave3Time = gameTimer.runTime;

        GameFinishedCanvas.SetActive(true);
        Time.timeScale = 0f;
        UnlockMouse();

        //Show Game Data
    }

    public void Wave2DifficultyAdjust()
    {
        //Put condition base on player Stats on prev waves (1)

        wave2Enemies[0].GetComponent<AttackStateBehaviour>().attackInIntervalsTime = new Vector2(MinimumInterval, MaxInterval);
    }

    public void Wave3DifficultyAdjust()
    {
        //Put condition base on player Stats on prev waves (1 and 2)

        _nuno_AttackManager.MinimumAttackDuration = 0.5f;
        _nuno_AttackManager.MaxAttackDuration = 1f;
        _nuno_AttackManager.stunDuration = 3f;
    }

    public void SaveGameData() //save after every Wave or death
    {
        PlayerPrefs.SetFloat("Total Run Time", TotalRunTime);
        PlayerPrefs.SetFloat("Total Damage Dealt", TotalDamageDealt);
        PlayerPrefs.SetFloat("Total Damage Taken", TotalDamageTaken);

        
    }

    public void Restart()
    {
        PlayerPrefs.SetFloat("Total Damage Count", PlayerPrefs.GetFloat("Total Death Count") + 1);
        SceneManager.LoadScene(SceneToLoad);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
        ClearPlayerPrefs();
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.SetFloat("PlayerID", 0);
        PlayerPrefs.SetInt("Mode", 0);
        PlayerPrefs.SetFloat("Total Run Time", 0);
        PlayerPrefs.SetFloat("Total Damage Dealt", 0);
        PlayerPrefs.SetFloat("Total Damage Taken", 0);
        PlayerPrefs.SetFloat("Total Death Count", 0);
    }
}
