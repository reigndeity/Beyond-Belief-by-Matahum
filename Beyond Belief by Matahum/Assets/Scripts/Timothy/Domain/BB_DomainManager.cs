using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class BB_DomainManager : MonoBehaviour
{
    public static BB_DomainManager instance;

    [Header("Domain Settings")]
    [HideInInspector]public BB_DomainSO selectedDomain;
    private List<BB_DomainSO> domainList = new List<BB_DomainSO>();
    private int currentSetIndex = 0;
    public GameObject domainArea;
    [HideInInspector] public bool isDomainComplete = false;
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

    [Header("Active Enemies")]
    public List<Enemy> enemyList = new List<Enemy>();
    public Transform enemyHolder;

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

    public void SpawnToDomainEntrance()
    {
        Player player = FindFirstObjectByType<Player>();
        player.transform.position = spawnPoint;
        player.transform.rotation = spawnRotation;
    }

    public void EnterDomain()
    {
        SceneManager.LoadScene("Balete Tree Domain");
    }

    public void StartDomain()
    {
        totalTimer = selectedDomain.totalTime;
        timerText.gameObject.SetActive(true); // Show timer
        StartCoroutine(TimerRoutine());
        StartCoroutine(DomainRoutine());
    }

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

        // Optional: handle time-up behavior
        Debug.Log("Time's up!");

        Defeat();
    }


    private IEnumerator DomainRoutine()
    {
        while (currentSetIndex < selectedDomain.enemySets.Count) // <-- POTENTIAL BUG: enemySets might not exist, check your SO script
        {
            var set = selectedDomain.enemySets[currentSetIndex];

            SpawnEnemySet(set);

            if (selectedDomain.spawnMode == DomainSpawnMode.TimeBased)
            {
                yield return new WaitForSeconds(set.delayBeforeNextWave);
            }
            else if (selectedDomain.spawnMode == DomainSpawnMode.ClearBased)
            {
                yield return new WaitUntil(() => enemyList.Count == 0);
            }

            currentSetIndex++;
        }

        Debug.Log("Domain completed!");
    }

    private void SpawnEnemySet(BB_EnemySet set)
    {
        foreach (var enemyData in set.enemyList)
        {
            for (int i = 0; i < enemyData.howManyToSpawn; i++)
            {
                Enemy enemy = Instantiate(enemyData.enemyToSpawn, GetRandomSpawnPosition(), Quaternion.identity, enemyHolder).GetComponent<Enemy>();
                enemyList.Add(enemy);

                enemy.OnDeath += () => StartCoroutine(DomainCompletion(enemy));
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(Random.Range(-10f, 10f), 2, Random.Range(-10f, 10f));
    }

    private void OnDrawGizmosSelected()
    {
        if (domainArea == null) return;

        // Make the gizmo red and slightly transparent
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);

        // Draw a filled sphere for visualization (radius = 10 to match Random.Range(-10, 10))
        Gizmos.DrawSphere(domainArea.transform.position + new Vector3(0f, 2f, 0f), 10f);

        // Optionally draw a wireframe so edges are clearer
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(domainArea.transform.position + new Vector3(0f, 2f, 0f), 10f);
    }



    private IEnumerator DomainCompletion(Enemy enemy)
    {
        Debug.Log("Namatay");
        enemyList.Remove(enemy); // <-- BUG FIX: This should be before checking count

        if (enemyList.Count == 0)
        {
            SpawnChest();
            isDomainComplete = true;

            domainCompletePopUp.SetActive(true);
            CanvasGroup canvasGroup = domainCompletePopUp.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            float duration = 1f;
            float timer = 0f;

            // Fade in
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(timer / duration);
                yield return null;
            }

            yield return new WaitForSeconds(3f);

            // Fade out
            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(timer / duration);
                yield return null;
            }

            domainCompletePopUp.SetActive(false);
        }
    }

    public void Defeat()
    {
        //Stop movement of all enemies and player


    }

    void SpawnChest()
    {
        // This will search ALL objects in the scene, even inactive ones
        BB_DomainClaimRewardsInteractable chest = Resources.FindObjectsOfTypeAll<BB_DomainClaimRewardsInteractable>()
            .FirstOrDefault(obj => obj.gameObject.scene.isLoaded);

        if (chest != null)
        {
            chest.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Chest object not found in scene.");
        }
    }

    public void ClaimPanel(BB_DomainSO domain)
    {
        claimRewardPanel.SetActive(true);

        foreach (BB_RewardSO rewards in domain.GetRewardsWithMultiplier())
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


    }

    public void ClaimRewards(BB_DomainSO domain)
    {
        foreach (var reward in domain.GetRewardsWithMultiplier())
        {
            reward.GiveReward();
            Debug.Log($"Received {reward.RewardQuantity()} {reward.RewardName()}");
        }
    }
}
