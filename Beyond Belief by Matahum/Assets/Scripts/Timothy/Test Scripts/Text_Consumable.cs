using UnityEngine;

public class Text_Consumable : MonoBehaviour, IUsable
{
    private Temp_PlayerStats _playerStats;
    private InventoryItem _item;

    void Start()
    {
        //_playerStats = FindAnyObjectByType<PlayerStats>();
        _item = GetComponent<InventoryItem>();
    }
    public void Use()
    {
        //if (_playerStats != null) _playerStats.p_currentHealth += 100;

        Debug.Log("Heal Player 100 points");

        _item.quantity--;

        ItemSlot slot = GetComponentInParent<ItemSlot>();
        slot.UpdateQuantityText();

        if (_item.quantity <= 0)
        {
            slot.ClearItem();
            Destroy(gameObject);
            slot.gameObject.SetActive(false);
        }
    }
}
