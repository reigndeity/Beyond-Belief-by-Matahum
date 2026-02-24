using UnityEngine;
using BlazeAISpace;
using UnityEngine.SceneManagement;

public class ArenaManagerScript : MonoBehaviour
{
    [Header("Arena Stats")]
    public float MinimumInterval = 0.5f; //change to private later
    public float MaxInterval = 3f; //change to private later
    public int CurrentWaveCount = 0;

    [Header("Scene")]
    public string SceneToLoad;

    [Header("Game Over")]
    public GameObject GameOverUI;

    [Header("Wave Holders")]
    public GameObject Wave1Holder;
    public GameObject Wave2Holder;
    public GameObject Wave3Holder;

    //Wave 2
    public GameObject[] wave2Enemies;

    //Wave 3
    public Nuno _nuno;
    public Nuno_AttackManager _nuno_AttackManager;

    public void Start()
    {
        Wave2DifficultyAdjust();
        CurrentWaveCount++;
        WaveCheck();
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
            CurrentWaveCount++;
            WaveCheck();
        }
    }

    public void CurrentWave()
    {
        
    }

    public void OnPlayerDeath()
    {
        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }


    public void GameFinished()
    {
        Debug.Log("Game Done Show Player Stats");
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

    public void Restart()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

}
