using System;
using UnityEngine;

public class Chest : Interactable
{
    [Header("Chest Contents")]
    public LootContent[] lootDrops;
    public bool isOpened = false;
    
    [Header("Loot Physics Settings")]
    public float explosionForce = 5f;         // how strong the items shoot out
    public Vector3 spawnOffset = Vector3.up * 0.5f; // offset above chest

    [Header("Animator")]
    private Animator animator;
    private GameObject chestParticle;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (isOpened)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            animator.CrossFade("Loot Chest_Opened", 0.1f);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Loot");
            animator.CrossFade("Loot Chest_Closed", 0.1f);
        }
    }


    public override void OnInteract()
    {
        if (lootDrops == null || lootDrops.Length == 0)
        {
            Debug.LogWarning("Chest has no drops or lootPrefab is missing.");
            return;
        }

        
        if(!isOpened)
        {
            isOpened = true;
            animator.CrossFade("Loot Chest_Opening", 0.1f);
        }
    }

    public void ShootOutLoot()
    {
        Vector3 chestPosition = transform.position + spawnOffset;

        foreach (var chestDrop in lootDrops)
        {
            // Spawn loot object
            LootDrops lootObj = Instantiate(chestDrop.lootPrefab, chestPosition, Quaternion.identity);
            lootObj.lootContent = chestDrop;
            lootObj.Initialize();

            Rigidbody rb = lootObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate random direction within a sphere
                Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f; // ensure it pops upward a bit

                // Apply explosion-like force
                rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
            }
        }
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void ShowParticle(GameObject particle)
    {
        chestParticle = Instantiate(particle, transform);
        chestParticle = particle;
        chestParticle.SetActive(true);
    }
    public void HideParticle()
    {
        chestParticle.SetActive(false);
    }
}
/*
[Serializable]
public class ChestContent
{
    public LootDrops lootPrefab;

    public R_ItemData itemData;
    public int level = 1;
    public bool randomPamana = false;
    public bool randomAgimat = false;
    public bool isGold = false;
    public bool randomGold = false;
    public int goldAmount;
    public Vector2 randomGoldMinMax;
}
*/