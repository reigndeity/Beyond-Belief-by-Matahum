using UnityEngine;

public class PamanaLootDrop : MonoBehaviour//, IPickable
{
    private PamanaSpawner pamanaSpawner;
    public PamanaUI pamanaUIScript;
    private Transform player;

    void Awake()
    {
    }

    void Start()
    {
        pamanaSpawner = FindFirstObjectByType<PamanaSpawner>();
        pamanaUIScript = pamanaSpawner.SpawnPamana(transform.gameObject);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        Destroy(transform.gameObject, 60f);
    }

    public void PickUp()
    {
        if (pamanaUIScript != null && pamanaSpawner != null)
        {
            pamanaSpawner.InsertToInventory(pamanaUIScript);
            Debug.Log($"Picked Up {pamanaUIScript.pamanaName}");
        }
    }
}
