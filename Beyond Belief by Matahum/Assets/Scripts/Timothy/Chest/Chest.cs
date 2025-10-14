using System;
using UnityEngine;

public class Chest : Interactable
{
    [Header("Chest Contents")]
    public LootContent[] lootDrops;
    public bool isOpened = false;

    [Header("Chest Settings")]
    [SerializeField] private bool isLewenriChest = false;  // ✅ mark Lewenri chests in Inspector

    [Header("Unique ID (auto-assigned if empty)")]
    [SerializeField, HideInInspector] private string chestId;
    public string ChestId => chestId;
    public bool IsLewenriChest => isLewenriChest;

    [Header("Loot Physics Settings")]
    public float explosionForce = 5f;
    public Vector3 spawnOffset = Vector3.up * 0.5f;

    [Header("Animator & Particles")]
    private Animator animator;
    [SerializeField] private GameObject chestParticlePrefab;
    private GameObject activeParticle;

    private void OnValidate()
    {
        // Automatically assign unique ID if none exists
        if (string.IsNullOrEmpty(chestId))
            chestId = Guid.NewGuid().ToString();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        ApplySavedState();
    }

    public void ApplySavedState()
    {
        if (animator == null) animator = GetComponent<Animator>();

        if (isOpened)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            animator.CrossFade("Loot Chest_Opened", 0.1f);
            HideParticle();
        }
        else
        {
            // If this is a Lewenri chest, respect ChestManager state
            if (isLewenriChest && !ChestManager.Instance.CanOpenLewenriChests)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                HideParticle();
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Loot");
                ShowParticle();
            }

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

        // 🚫 Block Lewenri chests if quest not yet complete
        if (isLewenriChest && !ChestManager.Instance.CanOpenLewenriChests)
        {
            Debug.Log("You feel a strange power sealing this chest...");
            return;
        }

        if (!isOpened)
        {
            isOpened = true;
            animator.CrossFade("Loot Chest_Opening", 0.1f);
            HideParticle();

            // 🧩 Prevent further interaction
            gameObject.layer = LayerMask.NameToLayer("Default");

            // If you have a collider used for detection, disable it
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

    }

    public void ShootOutLoot()
    {
        Vector3 chestPosition = transform.position + spawnOffset;

        foreach (var chestDrop in lootDrops)
        {
            LootDrops lootObj = Instantiate(chestDrop.lootPrefab, chestPosition, Quaternion.identity);
            lootObj.lootContent = chestDrop;
            lootObj.Initialize();

            Rigidbody rb = lootObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f;
                rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    // =====================================================
    // PARTICLE CONTROL
    // =====================================================

    public void ShowParticle()
    {
        if (chestParticlePrefab == null || activeParticle != null) return;

        activeParticle = Instantiate(chestParticlePrefab, transform);
        activeParticle.SetActive(true);
    }

    public void HideParticle()
    {
        if (activeParticle != null)
        {
            Destroy(activeParticle);
            activeParticle = null;
        }
    }
}
