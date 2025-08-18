using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BB_DomainManager : MonoBehaviour
{
    public static BB_DomainManager instance;

    [Header("Domain Settings")]
    [HideInInspector]public BB_DomainSO selectedDomain;
    private List<BB_DomainSO> domainList = new List<BB_DomainSO>();
    public int currentSetIndex = 0;
    [HideInInspector] public bool isDomainComplete = false;
    public bool hasFinishedSpawning = false;

    [Header("Going to OpenWorld from Domain")]
    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    [Header("UI References")]
    public BB_IconUIGroup iconGroupTemplate;
    public GameObject domainCompletePopUp;

    [Header("Timers")]
    public TextMeshProUGUI timerText;
    private float totalTimer;

    [Header("Defeated Properties")]
    public GameObject defeatedPanel;
    public Button defeatedButton;
    public bool isDefeated;

    [Header("Active Enemies")]
    public List<Enemy> enemyList = new List<Enemy>();
    [HideInInspector] public BB_EnemyHolderWrapper enemyHolder;

    [Header("Chest")]
    [HideInInspector]public GameObject chestObj;

    [Header("Claim Reward Properties")]
    public GameObject claimRewardPanel;
    public Transform rewardHolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
            domainList = new List<BB_DomainSO>(Resources.LoadAll<BB_DomainSO>("Domains")); // Loads all available domains
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SpawnToOpenWorldDomainEntrance()
    {
        Player player = FindFirstObjectByType<Player>();
        player.transform.position = spawnPoint;
        player.transform.rotation = spawnRotation;
    }

    public void EnterDomain()
    {
        //SceneManager.LoadScene("Balete Tree Domain");
        Loader.Load(4);
    }

    public void StartDomain()
    {
        StopAllCoroutines(); // Prevent old stuck routines
        enemyList.Clear();   // Clear leftover enemies
        currentSetIndex = 0; // Always reset before starting
        isDomainComplete = false;
        hasFinishedSpawning = false;
        totalTimer = selectedDomain.totalTime;
        timerText.gameObject.SetActive(true);
        StartCoroutine(TimerRoutine());
        StartCoroutine(DomainRoutine());
    }
    public void ResetDomain()
    {
        StopAllCoroutines();
        enemyList.Clear();
        currentSetIndex = 0;
        defeatedPanel.SetActive(false);
        timerText.gameObject.SetActive(false);
        claimRewardPanel.gameObject.SetActive(false);
        isDefeated = false;
    }

    #region DOMAIN ROUTINE
    private IEnumerator TimerRoutine()
    {
        while (totalTimer > 0 && !isDomainComplete)
        {
            totalTimer -= Time.deltaTime;
            if (totalTimer < 0) totalTimer = 0;

            // Format time as MM:SS
            int minutes = Mathf.FloorToInt(totalTimer / 60f);
            int seconds = Mathf.FloorToInt(totalTimer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

            yield return null; // wait 1 frame
        }
        if (!isDefeated && !isDomainComplete && totalTimer <= 0f)
        {
            Defeat();
        }
    }
    private IEnumerator DomainRoutine()
    {
        hasFinishedSpawning = false; // Reset at start
        currentSetIndex = 0;

        while (currentSetIndex < selectedDomain.enemySets.Count)
        {
            var set = selectedDomain.enemySets[currentSetIndex];

            SpawnEnemySet(set);

            if (selectedDomain.spawnMode == DomainSpawnMode.TimeBased)
            {
                yield return new WaitForSeconds(set.delayBeforeNextWave);
            }
            else if (selectedDomain.spawnMode == DomainSpawnMode.ClearBased)
            {
                // If this was the last wave in ClearBased mode, set the flag
                if (currentSetIndex == selectedDomain.enemySets.Count - 1)
                {
                    hasFinishedSpawning = true;
                    Debug.Log("ClearBased domain finished spawning all enemy sets!");
                }

                // Wait until player clears this wave
                yield return new WaitUntil(() => enemyList.Count == 0);     
            }

            currentSetIndex++;
        }

        // If not ClearBased, TimeBased mode will set the flag after loop ends
        if (selectedDomain.spawnMode == DomainSpawnMode.TimeBased)
        {
            hasFinishedSpawning = true;
            Debug.Log("TimeBased domain finished spawning all enemy sets!");
        }
    }
    private void SpawnEnemySet(BB_EnemySet set)
    {
        enemyHolder = FindFirstObjectByType<BB_EnemyHolderWrapper>();
        foreach (var enemyData in set.enemyList)
        {
            for (int i = 0; i < enemyData.howManyToSpawn; i++)
            {
                Enemy enemy = Instantiate(enemyData.enemyToSpawn, GetRandomSpawnPosition(), Quaternion.identity, enemyHolder.transform).GetComponent<Enemy>();
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                enemyStats.SetLevel(10 * selectedDomain.levelMultiplier); //Remove Comment when testing enemy levels. its working though

                enemyList.Add(enemy);

                enemy.OnDeath += () => StartCoroutine(DomainCompletion(enemy));
            }
        }
    }
    private Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(Random.Range(-10f, 10f), 2, Random.Range(-10f, 10f));
    }

    #endregion
    #region DEFEATED
    public void Defeat()
    {
        isDefeated = true;

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.PauseGame();

        StartCoroutine(DefeatPanel());
    }

    IEnumerator DefeatPanel()
    {
        // Gradually Show Panel
        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        
        defeatedPanel.SetActive(true);
        CanvasGroup canvasGroup = defeatedPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        CanvasGroup btnCanvasGroup = defeatedButton.GetComponent<CanvasGroup>();
        btnCanvasGroup.alpha = 0f;

        float duration = 1f;
        float timer = 0f;

        // Fade in (unscaled time)
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        // Wait without being affected by timeScale
        yield return new WaitForSecondsRealtime(1f);

        timerText.gameObject.SetActive(false);

        // Gradually Show Button
        uiGame.HideUI();
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            btnCanvasGroup.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(true);
    }
    #endregion
    #region CLAIM REWARDS

    private IEnumerator DomainCompletion(Enemy enemy)
    {
        Debug.Log("Namatay");
        enemyList.Remove(enemy); // <-- BUG FIX: This should be before checking count

        if (enemyList.Count == 0 && hasFinishedSpawning == true)
        {
            UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);

            isDomainComplete = true;

            domainCompletePopUp.SetActive(true);
            CanvasGroup domainCompleteCanvasGroup = domainCompletePopUp.GetComponent<CanvasGroup>();
            domainCompleteCanvasGroup.alpha = 0f;

            float duration = 1f;
            float timer = 0f;

            // Fade in Domain Complete Pop Up
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                domainCompleteCanvasGroup.alpha = Mathf.Clamp01(timer / duration);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);

            // Fade out Domain Complete Pop Up
            timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                domainCompleteCanvasGroup.alpha = 1f - Mathf.Clamp01(timer / duration);
                yield return null;
            }

            domainCompletePopUp.SetActive(false);
            timerText.gameObject.SetActive(false);
            claimRewardPanel.SetActive(true);
            uiGame.HideUI();
            uiGame.PauseGame();

            PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
            playerCam.SetCursorVisibility(true);

            ClaimPanel(selectedDomain);
            CanvasGroup claimRewardCanvasGroup = claimRewardPanel.GetComponent<CanvasGroup>();
            claimRewardCanvasGroup.alpha = 0f;

            timer = 0f;

            // Fade in Claim Rewards
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                claimRewardCanvasGroup.alpha = Mathf.Clamp01(timer / duration);
                yield return null;
            }
        }
    }
    public void ClaimPanel(BB_DomainSO domain)
    {
        foreach (Transform child in rewardHolder)
        {
            BB_IconUIGroup iconUIGroup = child.GetComponent<BB_IconUIGroup>();
            if (iconUIGroup != null)
                Destroy(iconUIGroup.gameObject);
        }

        int rewardArrayIndex = RewardArrayIndex(domain, domain.levelMultiplier);

        foreach (BB_RewardSO rewards in domain.rewards[rewardArrayIndex].rewards)
        {
            BB_IconUIGroup rewardUI = Instantiate(iconGroupTemplate, rewardHolder);
            rewardUI.gameObject.SetActive(true);

            rewardUI.backgroundImage.sprite = rewards.RewardBackground();
            rewardUI.icon.sprite = rewards.RewardIcon();
            rewardUI.iconName.text = $"{rewards.RewardQuantity().ToString()}  {rewards.RewardName()}";
            rewardUI.quantity.text = rewards.RewardQuantity().ToString();

            RectTransform iconRect = rewardUI.gameObject.GetComponent<RectTransform>();
            iconRect.localScale = new Vector2(2f, 2f);
        }

        SaveSystemManager();

        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(true);
    }

    public void ClaimRewards(BB_DomainSO domain)
    {
        int rewardArrayIndex = RewardArrayIndex(domain, domain.levelMultiplier);

        foreach (var reward in domain.rewards[rewardArrayIndex].rewards)
        {
            reward.GiveReward();
            Debug.Log($"Received {reward.RewardQuantity()} {reward.RewardName()}");
        }

        if (!domain.isDomainClearedForQuest)
        {
            domain.levelAccessIndex++;
            domain.isDomainClearedForQuest = true;
        }
        else if (domain.levelAccessIndex == BB_DomainDetailsUI.instance.levelAccessIndex)
        {
            if (domain.levelAccessIndex < 5)
            {
                Debug.Log("Increase Index Level");
                domain.levelAccessIndex++;
            }
        }
        
        BB_QuestManager.Instance?.UpdateMissionProgress(domain.questTargetID, 1);
    }

    public int RewardArrayIndex(BB_DomainSO domain, int rewardArrayIndex)
    {      
        if (domain.levelMultiplier == 1)
        {
            if (!domain.isDomainClearedForQuest)    
                rewardArrayIndex = 0;          
            else rewardArrayIndex = 1;
        }
        else rewardArrayIndex = domain.levelMultiplier;

        return rewardArrayIndex;
    }
    #endregion

    async void SaveSystemManager()
    {
        await SaveManager.Instance.SaveSystemsAsync("slot_01", false, "Equipment.Main", "Inventory.Main", "Player.Stats");
    }
}
