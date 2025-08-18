using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;

public class BB_DomainDetailsUI : MonoBehaviour
{
    public static BB_DomainDetailsUI instance;

    [Header("Domain Selection")]
    public Transform domainSelectionHolder;
    public int levelAccessIndex;

    [Header("Domain Details")]
    public Image domainImage;
    public TextMeshProUGUI domainTitleText;
    public TextMeshProUGUI domainBodyText;
    public Transform creatureHolder;
    public Transform rewardHolder;
    public BB_IconUIGroup iconUIGroup;
    private bool isDomainButtonsSetUp = false;

    [Header("Domain Selected Indicator")]
    public Sprite defaultSprite;
    public Sprite selectedSprite; // Assign this manually or load it
    private Button currentlySelectedButton;
    private Image currentlySelectedImage;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    public void OnOpenDomainDetails(bool shouldApplyMultiplier = false)
    {
        for (int i = 0; i < domainSelectionHolder.childCount; i++)
        {
            BB_DomainSelectionTemplate domainSelectionTemplate = domainSelectionHolder.GetChild(i).GetComponent<BB_DomainSelectionTemplate>();
            domainSelectionTemplate.domainTitle.text = BB_DomainManager.instance.selectedDomain.domainName;

            Button domainBtn = domainSelectionHolder.GetChild(i).GetComponent<Button>();
            Image domainImg = domainBtn.GetComponent<Image>();

            int level = i;
            if (level == 0) level = 1;

            if (!isDomainButtonsSetUp)
            {
                domainBtn.onClick.AddListener(() => OnDomainButtonClicked(BB_DomainManager.instance.selectedDomain, domainBtn, domainImg, level, shouldApplyMultiplier));
            }
          
            // Only unlock button if previous levels are cleared
            if (level <= BB_DomainManager.instance.selectedDomain.levelAccessIndex)
            {
                domainBtn.gameObject.SetActive(true);
                domainBtn.interactable = true;
            }
            else
            {
                domainBtn.gameObject.SetActive(true);
                domainBtn.interactable = false;
            }
        }
        isDomainButtonsSetUp = true;

        // Toggle visibility depending on whether domain is cleared
        if (!BB_DomainManager.instance.selectedDomain.isDomainClearedForQuest)        
            for (int i = 0; i < domainSelectionHolder.childCount; i++)            
                domainSelectionHolder.GetChild(i).gameObject.SetActive(i == 0);                   
        else        
            for (int i = 0; i < domainSelectionHolder.childCount; i++)
            domainSelectionHolder.GetChild(i).gameObject.SetActive(i != 0);       

        // Auto-click the first *active* button
        for (int i = 0; i < domainSelectionHolder.childCount; i++)
        {
            if (domainSelectionHolder.GetChild(i).gameObject.activeSelf)
            {
                domainSelectionHolder.GetChild(i).GetComponent<Button>().onClick.Invoke();
                break;
            }
        }
    }
    private void OnDomainButtonClicked(BB_DomainSO domain, Button clickedButton, Image clickedImage, int level, bool shouldApplyMultiplier = false)
    {
        levelAccessIndex = level;
        // Reset previous button to default sprite
        if (currentlySelectedImage != null)
        {
            currentlySelectedImage.sprite = defaultSprite;
        }

        // Set new button sprite to selected
        currentlySelectedButton = clickedButton;
        currentlySelectedImage = clickedImage;
        currentlySelectedImage.sprite = selectedSprite;

        // Show quest details
        ShowDomainDetails(domain, level, shouldApplyMultiplier);
    }
    public void ShowDomainDetails(BB_DomainSO domain, int level, bool shouldApplyMultiplier = false)
    {
        domainImage.sprite = domain.domainImage;
        domainTitleText.text = domain.domainName;
        domainBodyText.text = domain.domainDescription;
        if(shouldApplyMultiplier == true ) domain.levelMultiplier = level;

        //Remove creature icons
        foreach (Transform child in creatureHolder)
        {
            BB_IconUIGroup iconUIGroup = child.GetComponent<BB_IconUIGroup>();
            if (iconUIGroup != null)
                Destroy(iconUIGroup.gameObject);         
        }

        //Remove reward icons
        foreach (Transform child in rewardHolder)
        {
            BB_IconUIGroup iconUIGroup = child.GetComponent<BB_IconUIGroup>();
            if (iconUIGroup != null)       
                Destroy(iconUIGroup.gameObject);
        }

        // Populate creature icons
        Dictionary<GameObject, int> enemyCounts = new Dictionary<GameObject, int>();

        foreach (BB_EnemySet wave in domain.enemySets)
        {
            foreach (BB_DomainEnemy enemyData in wave.enemyList)
            {
                if (enemyCounts.ContainsKey(enemyData.enemyToSpawn))
                {
                    enemyCounts[enemyData.enemyToSpawn] += enemyData.howManyToSpawn;
                }
                else
                {
                    enemyCounts[enemyData.enemyToSpawn] = enemyData.howManyToSpawn;
                }
            }
        }

        // Now create icons for each unique enemy
        foreach (var kvp in enemyCounts)
        {
            GameObject enemyPrefab = kvp.Key;
            int totalCount = kvp.Value;

            // Create icon UI
            BB_IconUIGroup icon = Instantiate(iconUIGroup, creatureHolder);

            icon.iconName.text = $"{totalCount} {enemyPrefab.name}";
            //icon.icon.sprite =  Enemy.enemyIcon;
            icon.quantity.text = totalCount.ToString();
        }

        // Populate Reward Icons
        int rewardArrayIndex = BB_DomainManager.instance.RewardArrayIndex(domain, domain.levelMultiplier);

        foreach (BB_RewardSO rewards in domain.rewards[rewardArrayIndex].rewards)
        {
            // Create icon UI
            BB_IconUIGroup rewardUI = Instantiate(iconUIGroup, rewardHolder);
            rewardUI.gameObject.SetActive(true);

            rewardUI.backgroundImage.sprite = rewards.RewardBackground();
            rewardUI.icon.sprite = rewards.RewardIcon();
            rewardUI.iconName.text = $"{rewards.RewardQuantity().ToString()}  {rewards.RewardName()}";
            rewardUI.quantity.text = rewards.RewardQuantity().ToString();
        }
    }
}
