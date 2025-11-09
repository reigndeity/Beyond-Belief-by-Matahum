using UnityEngine;

public class BananaTreeEasterEgg : Interactable
{
    public LootContent mutyaNgSaging;

    public Transform spawnPoint;
    public bool isInteracted;

    [Header("Loot Physics Settings")]
    public float explosionForce = 5f;
    public Vector3 spawnOffset = Vector3.up * 1f;
    



    private void Start()
    {
        if (isInteracted == true)
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
        isInteracted = true;
    }
}
