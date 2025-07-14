using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class CharDet_PamanaInventory : MonoBehaviour
{
    [Header("Item Slot Prefab and Main Inventory")]
    public GameObject slotPrefab;
    public Inventory mainInventory;

    [Header("Character Details Pamana Contents")]
    public Transform pamanaInventory;
    public List<ItemSlot> mainInventorySlots = new List<ItemSlot>();
    public List<InventoryItem> allItems = new();
    private int slotNumber = 1;

    private List<ItemSlot> storedSlots = new List<ItemSlot>();
    private List<InventoryItem> storedItems = new();

    private void Start()
    {
        //Invoke("RefreshUI",0.5f);
    }
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

}
