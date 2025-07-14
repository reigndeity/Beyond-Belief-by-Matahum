using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PamanaInventory : MonoBehaviour
{
    [Header("Default Pamana Slots")]
    public ItemSlot diwataSlot;
    public ItemSlot lihimSlot;
    public ItemSlot SalamangkeroSlot;

    [Header("All Pamana Slots(For when you want more than 3 slots)")]
    public List<ItemSlot> pamanaSlots = new();

    private Dictionary<ItemSlot, PamanaType> slotToType = new();
    private Dictionary<ItemSlot, (PamanaStat main, PamanaStat sub)> storedStats = new();

    private PamanaSetPieces setPiece;
    //private PlayerStats player;
    private Temp_PlayerStats player;

    private void Awake()
    {
        setPiece = GetComponent<PamanaSetPieces>();

        slotToType[diwataSlot] = PamanaType.Diwata;
        slotToType[lihimSlot] = PamanaType.Lihim_ng_Karunungan;
        slotToType[SalamangkeroSlot] = PamanaType.Salamangkero;
    }

    private void Start()
    {
        player = FindFirstObjectByType<Temp_PlayerStats>();

        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForEndOfFrame();
        InitialApplyStats();
    }

    public void AddPamanaSlot(ItemSlot newSlot, PamanaType type)
    {
        if (!pamanaSlots.Contains(newSlot))
        {
            pamanaSlots.Add(newSlot);
            slotToType[newSlot] = type;
            newSlot.OnItemChanged += HandleItemChanged;
        }
    }

    /*public void AddDiwataSlot()
    {
        ItemSlot newSlot = Instantiate(slotPrefab, this.transform);
        AddPamanaSlot(newSlot, PamanaType.Diwata);
    }
    public void AddLihimSlot()
    {
        ItemSlot newSlot = Instantiate(slotPrefab, this.transform);
        AddPamanaSlot(newSlot, PamanaType.Lihim_ng_Karunungan);
    }
    public void AddSalamangkeroSlot()
    {
        ItemSlot newSlot = Instantiate(slotPrefab, this.transform);
        AddPamanaSlot(newSlot, PamanaType.Salamangkero);
    }*/

    public ItemSlot GetSlotByType(PamanaType type)
    {
        foreach (var kvp in slotToType)
        {
            if (kvp.Value == type)
                return kvp.Key;
        }
        return null;
    }

    public void InitialApplyStats()
    {
        foreach (var slot in pamanaSlots)
        {
            if (slot.GetComponentInChildren<InventoryItem>())
            {
                ApplyStats(slot, slot.GetComponentInChildren<InventoryItem>());
                Debug.Log($"Applied Stats from {slotToType[slot]}");
            }
        }
    }

    private void HandleItemChanged(ItemSlot slot, InventoryItem oldItem, InventoryItem newItem)
    {
        if (oldItem != null)
        {
            setPiece.UnequipPamana(oldItem.GetComponent<PamanaUI>().assignedPamana);
            RemoveStats(slot);
        }
        if (newItem != null) ApplyStats(slot, newItem);
    }

    private void ApplyStats(ItemSlot slot, InventoryItem item)
    {
        if (item == null) return;

        PamanaUI pamanaUI = item.GetComponent<PamanaUI>();
        if (pamanaUI == null)
        {
            Debug.LogError("PamanaUI component is missing from the dropped item!");
            return;
        }

        pamanaUI.isEquipped = true;

        Pamana pamana = pamanaUI.assignedPamana;
        if (pamana == null)
        {
            Debug.LogError("assignedPamana is null in PamanaUI!");
            return;
        }

        SaveStats(slot, pamana.mainStat, pamana.subStat);

        player.ApplyStat(pamana.mainStat, pamana.mainStat.value);
        player.ApplyStat(pamana.subStat, pamana.subStat.value);

        setPiece.EquipPamana(pamana);

        if (slotToType.TryGetValue(slot, out PamanaType slotType))
        {
            if (slotType != pamana.type)
            {
                Debug.LogWarning($"Equipped {pamana.type} to {slotType} slot — mismatch!");
            }
        }
    }

    private void RemoveStats(ItemSlot slot)
    {
        if (storedStats.TryGetValue(slot, out var stats))
        {
            PamanaUI pamanaUI = slot.GetComponentInChildren<PamanaUI>();
            if (pamanaUI != null) pamanaUI.isEquipped = false;

            if (stats.main != null) player.RemoveStat(stats.main, stats.main.value);
            if (stats.sub != null) player.RemoveStat(stats.sub, stats.sub.value);

            storedStats.Remove(slot);
        }
    }

    private void SaveStats(ItemSlot slot, PamanaStat main, PamanaStat sub)
    {
        storedStats[slot] = (main, sub);
    }
}