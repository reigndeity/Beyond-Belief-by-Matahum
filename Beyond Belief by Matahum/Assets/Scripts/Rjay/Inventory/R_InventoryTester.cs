using UnityEngine;

public class R_InventoryTester : MonoBehaviour
{
    public R_Inventory inventory;
    public R_ItemData testItem;
    public R_ItemData testItem2;
    public int amount = 1;
    public R_InventoryUI inventoryUI;

    private float keyCooldown = 0.3f;
    private float nextPressTime = 0f;

    [Header("Pamana Spawning Reference")]
    [SerializeField] private R_ItemData[] pamanaTemplates;
    [SerializeField] private R_PamanaVisualConfig visualConfig;
    [SerializeField] private int simulatedPlayerLevel = 1;
    private void Update()
    {
        if (Time.time >= nextPressTime && Input.GetKeyDown(KeyCode.T))
        {
            if (inventory != null && testItem != null)
            {
                inventory.AddItem(testItem, amount);
                inventory.AddItem(testItem2, amount);
                inventoryUI.RefreshUI();
                Debug.Log($"Added {amount} x {testItem.itemName} (Total: {GetTotalAmount(testItem)})");
                Debug.Log($"Added {amount} x {testItem2.itemName} (Total: {GetTotalAmount(testItem2)})");

                nextPressTime = Time.time + keyCooldown;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            R_PamanaSpawnerUtility.SpawnRandomPamanaFromPool(pamanaTemplates, inventory, inventoryUI, simulatedPlayerLevel, visualConfig);
        }
    }

    private int GetTotalAmount(R_ItemData item)
    {
        int total = 0;
        foreach (var slot in inventory.items)
        {
            if (slot.itemData == item)
                total += slot.quantity;
        }
        return total;
    }
}
