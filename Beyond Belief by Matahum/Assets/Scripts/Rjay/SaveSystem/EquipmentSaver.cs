using UnityEngine;
using SaveSystemDTO;

[DefaultExecutionOrder(-40)] // after InventorySaver
[DisallowMultipleComponent]
public class EquipmentSaver : MonoBehaviour, ISaveable
{
    [SerializeField] private Player player;         // drag your Player
    [SerializeField] private R_Inventory inventory; // drag the same Player's R_Inventory

    void Awake()
    {
        if (!player)    player    = GetComponentInParent<Player>();
        if (!inventory) inventory = GetComponentInParent<R_Inventory>();
        SaveManager.Instance.Register(this);
    }

    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Equipment.Main";

    // Ensure anything we save has a UID (important for first-time equips)
    string EnsureUid(R_InventoryItem item)
    {
        if (item == null) return null;
        if (string.IsNullOrEmpty(item.runtimeID))
            item.runtimeID = System.Guid.NewGuid().ToString();
        return item.runtimeID;
    }

    public string CaptureJson()
    {
        var dto = new EquippedDTO
        {
            pamanaDiwataUid       = EnsureUid(player.GetEquippedPamana(R_PamanaSlotType.Diwata)),
            pamanaLihimUid        = EnsureUid(player.GetEquippedPamana(R_PamanaSlotType.Lihim)),
            pamanaSalamangkeroUid = EnsureUid(player.GetEquippedPamana(R_PamanaSlotType.Salamangkero)),
            agimatSlot1Uid        = EnsureUid(player.GetEquippedAgimat(1)),
            agimatSlot2Uid        = EnsureUid(player.GetEquippedAgimat(2)),
        };
        return JsonUtility.ToJson(dto, false);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json) || player == null || inventory == null) return;

        var dto = JsonUtility.FromJson<EquippedDTO>(json);
        if (dto == null) return;

        // Pamana
        EquipPamanaByUid(R_PamanaSlotType.Diwata,       dto.pamanaDiwataUid);
        EquipPamanaByUid(R_PamanaSlotType.Lihim,        dto.pamanaLihimUid);
        EquipPamanaByUid(R_PamanaSlotType.Salamangkero, dto.pamanaSalamangkeroUid);

        // Agimat
        EquipAgimatByUid(1, dto.agimatSlot1Uid);
        EquipAgimatByUid(2, dto.agimatSlot2Uid);
    }

    // -------- helpers --------
    void EquipPamanaByUid(R_PamanaSlotType slot, string uid)
    {
        if (string.IsNullOrEmpty(uid)) { player.UnequipPamana(slot); return; }
        var match = FindByUid(uid);
        if (match != null) player.EquipPamana(match);
        else Debug.LogWarning($"[EquipmentSaver] Pamana UID not found: {uid}");
    }

    void EquipAgimatByUid(int slotIndex, string uid)
    {
        if (string.IsNullOrEmpty(uid)) { player.UnequipAgimat(slotIndex); return; }
        var match = FindByUid(uid);
        if (match != null) player.EquipAgimat(match, slotIndex);
        else Debug.LogWarning($"[EquipmentSaver] Agimat UID not found: {uid}. Inventory count: {inventory.items.Count}");
    }

    R_InventoryItem FindByUid(string uid)
    {
        if (inventory?.items == null) return null;
        foreach (var it in inventory.items)
            if (it != null && it.runtimeID == uid) return it;
        return null;
    }
}
