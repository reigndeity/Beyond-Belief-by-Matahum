using UnityEngine;

public class R_GeneralItemSpawner : MonoBehaviour
{
    public static R_GeneralItemSpawner instance;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private R_Inventory inventory;
    [SerializeField] private R_InventoryUI inventoryUI;

    [Header("Pamana Spawning Reference")]
    [SerializeField] public R_ItemData[] pamanaTemplates;
    [SerializeField] private R_PamanaVisualConfig visualConfig;

    [Header("Agimat Spawning Reference")]
    [SerializeField] public R_ItemData[] agimatTemplates;
    [SerializeField] private R_AgimatVisualConfig agimatVisualConfig;

    public int simulatedPlayerLevel = 1;

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

    // private void Update()
    // {
    //     simulatedPlayerLevel = playerStats.currentLevel;
    // }

    #region PAMANA SPAWNING
    public void SpawnRandomPamanaFromTemplate(int level = 10)
    {
        R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(pamanaTemplates, inventory, inventoryUI, level, visualConfig);
    }

    public void SpawnSinglePamana(R_ItemData[] itemData, int level = 1)
    {
        R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(itemData, inventory, inventoryUI, level, visualConfig);
    }
    #endregion

    #region AGIMAT SPAWNING
    public void SpawnSingleAgimat(R_ItemData[] itemData, int level = 1)
    {
        R_AgimatSpawnerUtility.SpawnRandomAgimatFromPool(itemData, inventory, inventoryUI, level, agimatVisualConfig);
    }

    public void SpawnRandomAgimatFromTemplate(int level = 10)
    {
        R_AgimatSpawnerUtility.SpawnRandomAgimatFromPool(agimatTemplates, inventory, inventoryUI, level, agimatVisualConfig);
    }
    #endregion

    public void SetSimulatedLevel(int level)
    {
        simulatedPlayerLevel = Mathf.Max(1, level); // enforce minimum level 1
    }
}
