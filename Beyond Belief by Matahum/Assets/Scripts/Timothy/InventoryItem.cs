using UnityEngine;
using System;
using UnityEngine.UI;

public enum ItemType
{
    Any,
    Consumable,
    Material,
    Misc,
    Pamana,
    Agimat
}

[System.Serializable]
public class InventoryItem : MonoBehaviour
{
    public string itemName;
    public string itemID;
    public Sprite icon;
    public ItemType itemType;

    public bool isStackable = false; //Is it Stackable? More 
    public int quantity = 1; //If Stackable
    public bool isUsable = false; //Is it usable like potions and food
    public bool isEquippable = false; //Is it Equippable? This is for Agimat, Pamana, and Equippable Misc Items

    public event Action OnQuantityChanged;

    private void Start()
    {
        //SetItemDetails(itemName, itemID, icon, itemType);
    }
    public void SetQuantity(int newQuantity)
    {
        quantity = newQuantity;
        OnQuantityChanged?.Invoke(); // Notify listeners
    }

    public void SetItemDetails(string name, string ID, Sprite image, ItemType type)
    {
        itemName = name;
        itemID = ID;
        icon = image;
        itemType = type;

        gameObject.name = itemName;
        gameObject.GetComponent<Image>().sprite = icon;
    }
}