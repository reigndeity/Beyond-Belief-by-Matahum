using UnityEngine;

public class R_GeneralItemSpawner : MonoBehaviour
{
    public static R_GeneralItemSpawner instance;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private R_Inventory inventory;
    [SerializeField] private R_InventoryUI inventoryUI;

    [Header("Pamana Spawning Reference")]
    [SerializeField] private R_ItemData[] pamanaTemplates;
    [SerializeField] private R_PamanaVisualConfig visualConfig;

    [Header("Agimat Spawning Reference")]
    [SerializeField] private R_ItemData[] agimatTemplates;
    [SerializeField] private R_AgimatVisualConfig agimatVisualConfig;

    [SerializeField] private int simulatedPlayerLevel = 1;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {

        simulatedPlayerLevel = playerStats.currentLevel;

        // Testing Purposes
        if (Input.GetKeyDown(KeyCode.P))
        {
            R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(pamanaTemplates, inventory, inventoryUI, simulatedPlayerLevel, visualConfig);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {

        }
    }

    #region PAMANA SPAWNING
    public void SpawnRandomPamanaFromTemplate()
    {
        R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(pamanaTemplates, inventory, inventoryUI, simulatedPlayerLevel, visualConfig);
    }

    public void SpawnSinglePamana(R_ItemData[] itemData)
    {
        R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(itemData, inventory, inventoryUI, simulatedPlayerLevel, visualConfig);
    }
    #endregion

    #region AGIMAT SPAWNING
    public void SpawnRandomAgimatFromTemplate()
    {
        R_AgimatSpawnerUtility.SpawnRandomAgimatFromPool(agimatTemplates, inventory, inventoryUI, simulatedPlayerLevel, agimatVisualConfig);
    }
    #endregion
}
