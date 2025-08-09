using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BB_DomainManager : MonoBehaviour
{
    public static BB_DomainManager instance;

    [Header("Domain Settings")]
    [HideInInspector]public BB_DomainSO selectedDomain;
    private List<BB_DomainSO> domainList = new List<BB_DomainSO>();
    private int currentSetIndex = 0;

    [Header("UI References")]
    public BB_IconUIGroup iconGroupTemplate;
    public GameObject domainCompletePopUp;

    [Header("Timers")]
    private float totalTimer;

    [Header("Active Enemies")]
    public List<EnemyStats> enemyList = new List<EnemyStats>();

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

    public void EnterDomain()
    {
        SceneManager.LoadScene("Balete Tree Domain");
    }

    public void StartDomain()
    {
        totalTimer = selectedDomain.totalTime;
        StartCoroutine(DomainRoutine());
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
                EnemyStats enemy = Instantiate(enemyData.enemyToSpawn, GetRandomSpawnPosition(), Quaternion.identity, transform).GetComponent<EnemyStats>();
                enemyList.Add(enemy);

                BB_DomainEnemyDeathHandler deathHandler = enemy.gameObject.AddComponent<BB_DomainEnemyDeathHandler>();
                deathHandler.onDeath += () => StartCoroutine(DomainCompletion(enemy));
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(Random.Range(-10f, 10f), 2, Random.Range(-10f, 10f));
    }

    private IEnumerator DomainCompletion(EnemyStats enemy)
    {
        enemyList.Remove(enemy); // <-- BUG FIX: This should be before checking count

        if (enemyList.Count == 0)
        {
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

    public void ClaimPanel(BB_DomainSO domain, GameObject completionPanel, Transform rewardsHolder)
    {
        completionPanel.SetActive(true);

        foreach (BB_RewardSO rewards in domain.rewards)
        {
            BB_IconUIGroup icon = Instantiate(iconGroupTemplate, rewardsHolder);
            icon.iconName.text = selectedDomain.name;
            icon.icon.sprite = rewards.RewardIcon();
            icon.quantity.text = rewards.RewardQuantity().ToString();

            RectTransform iconRect = icon.gameObject.GetComponent<RectTransform>();
            iconRect.localScale = new Vector2(2f, 2f);
        }
    }

    public void ClaimRewards(BB_DomainSO domain)
    {
        foreach (var reward in domain.rewards)
        {
            reward.GiveReward();
        }
    }
}
