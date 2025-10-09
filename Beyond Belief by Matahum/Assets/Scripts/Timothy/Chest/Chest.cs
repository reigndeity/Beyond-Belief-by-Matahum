using System;
using UnityEngine;

public class Chest : Interactable
{
    [Header("Chest Contents")]
    public ChestContent[] chestDrops;
    
    [Header("Loot Physics Settings")]
    public float explosionForce = 5f;         // how strong the items shoot out
    public float upwardModifier = 1f;         // how much upward push they get
    public Vector3 spawnOffset = Vector3.up * 0.5f; // offset above chest

    [Header("Animator")]
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    public override void OnInteract()
    {
        if (chestDrops == null || chestDrops.Length == 0)
        {
            Debug.LogWarning("Chest has no drops or lootPrefab is missing.");
            return;
        }

        Debug.Log("Interacted with Chest");
        animator.CrossFade("Loot Chest_Opening", 0.1f);
    }

    public void ShootOutLoot()
    {
        Vector3 chestPosition = transform.position + spawnOffset;

        foreach (var chestDrop in chestDrops)
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
        Destroy(this);
    }
}

[Serializable]
public class ChestContent
{
    public LootDrops lootPrefab;

    public R_ItemData itemData;
    public bool randomPamana = false;
    public bool randomAgimat = false;
    public bool isGold = false;
    public bool randomGold = false;
    public int goldAmount;
    public Vector2 randomGoldMinMax;
}
