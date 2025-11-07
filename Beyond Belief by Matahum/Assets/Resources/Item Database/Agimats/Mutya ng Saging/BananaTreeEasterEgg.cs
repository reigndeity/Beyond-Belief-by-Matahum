using UnityEngine;

public class BananaTreeEasterEgg : Interactable
{
    public LootContent mutyaNgSaging;

    public Transform spawnPoint;

    [Header("Loot Physics Settings")]
    public float explosionForce = 5f;
    public Vector3 spawnOffset = Vector3.up * 1f;

    public bool IsInteractedAlready()
    {
        return PlayerPrefs.GetInt("BananaTree_IsInteracted", 0) == 1;
    }

    private void Start()
    {
        if (IsInteractedAlready())
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            this.enabled = false;
        }
    }

    public override void OnInteract()
    {
        ShootOutLoot();
    }

    public void ShootOutLoot()
    {
        LootContent lootCopy = mutyaNgSaging.Clone();
        lootCopy.level = 50;
        LootDrops lootObj = Instantiate(lootCopy.lootPrefab, spawnPoint.position, Quaternion.identity);
        lootObj.lootContent = lootCopy;
        lootObj.Initialize();

        Rigidbody rb = lootObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
            randomDir.y = Mathf.Abs(randomDir.y) + 0.5f;
            rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
        }

        gameObject.layer = LayerMask.NameToLayer("Default");

        PlayerPrefs.SetInt("BananaTree_IsInteracted", 1);
        PlayerPrefs.Save();
    }
}
