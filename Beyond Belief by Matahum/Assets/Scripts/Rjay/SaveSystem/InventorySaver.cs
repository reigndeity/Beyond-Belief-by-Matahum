using System;
using System.Collections.Generic;
using UnityEngine;
using SaveSystemDTO;   // from InventoryDTOs.cs
using Object = UnityEngine.Object; // resolve ambiguity

[DefaultExecutionOrder(-50)] // restore inventory before equipment
[DisallowMultipleComponent]
public class InventorySaver : MonoBehaviour, ISaveable
{
    [SerializeField] private R_Inventory inventory; // drag your Player's R_Inventory

    void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<R_Inventory>();
        if (!inventory) inventory = FindFirstObjectByType<R_Inventory>();
        SaveManager.Instance.Register(this);
    }

    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Inventory.Main";

    public string CaptureJson()
    {
        var list = new List<InvItemDTO>();

        if (inventory != null && inventory.items != null)
        {
            foreach (var slot in inventory.items)
            {
                if (slot == null || slot.itemData == null || slot.quantity <= 0) continue;

                var data = slot.itemData;

                if (data.isStackable)
                {
                    var dto = new InvItemDTO
                    {
                        uid         = null,
                        count       = slot.quantity,
                        soType      = data.GetType().AssemblyQualifiedName,
                        soJson      = JsonUtility.ToJson(data, false),
                        displayName = data.itemName
                    };
                    list.Add(dto);
                }
                else
                {
                    if (string.IsNullOrEmpty(slot.runtimeID))
                        slot.runtimeID = Guid.NewGuid().ToString();

                    for (int i = 0; i < slot.quantity; i++)
                    {
                        var dto = new InvItemDTO
                        {
                            uid         = (i == 0) ? slot.runtimeID : Guid.NewGuid().ToString(),
                            count       = 1,
                            soType      = data.GetType().AssemblyQualifiedName,
                            soJson      = JsonUtility.ToJson(data, false),
                            displayName = data.itemName
                        };

                        // ✅ Save pamanaData if this is a Pamana
                        if (data.itemType == R_ItemType.Pamana && data.pamanaData != null)
                        {
                            dto.pamanaJson = JsonUtility.ToJson(data.pamanaData, false);
                        }

                        list.Add(dto);
                    }
                }
            }
        }

        var dtoWrapper = new InventoryDTO { items = list.ToArray() };
        return JsonUtility.ToJson(dtoWrapper, true);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json) || inventory == null) return;

        var dto = JsonUtility.FromJson<InventoryDTO>(json);
        if (dto?.items == null) return;

        inventory.items.Clear();

        var assigned = new HashSet<string>();

        foreach (var it in dto.items)
        {
            var itemData = RebuildItemData(it, it.soType, it.soJson);
            if (itemData == null)
            {
                Debug.LogWarning($"[InventorySaver] Rebuild failed: {it.displayName} ({it.soType})");
                continue;
            }

            if (itemData.isStackable)
            {
                inventory.AddItem(itemData, Mathf.Max(1, it.count));
            }
            else
            {
                int count = Mathf.Max(1, it.count);
                for (int i = 0; i < count; i++)
                {
                    var perItemData = (count == 1) ? itemData : RebuildItemData(it, it.soType, it.soJson);
                    inventory.AddItem(perItemData, 1);

                    var slot = FindNextUnassignedInstance(inventory, perItemData, assigned);
                    if (slot != null)
                    {
                        // ✅ Always preserve saved UID for EquipmentSaver
                        var uidToUse = (i == 0 && !string.IsNullOrEmpty(it.uid))
                                       ? it.uid
                                       : Guid.NewGuid().ToString();

                        slot.runtimeID = uidToUse;
                        assigned.Add(uidToUse);
                    }
                }
            }
        }
    }

    static R_ItemData RebuildItemData(InvItemDTO dto, string typeName, string dataJson)
    {
        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(dataJson)) return null;

        var type = System.Type.GetType(typeName);
        if (type == null) return null;

        var instance = ScriptableObject.CreateInstance(type) as R_ItemData;
        if (!instance) return null;

        JsonUtility.FromJsonOverwrite(dataJson, instance);

        // Lookup original from ItemRegistry
        if (ItemRegistry.TryGetByAnyId(instance.itemName, out var original))
        {
            // Restore base visuals + description
            instance.itemIcon = original.itemIcon;
            instance.description = original.description;

            switch (instance.itemType)
            {
                case R_ItemType.Pamana:
                    instance.set = original.set;
                    instance.pamanaSlot = original.pamanaSlot;
                    instance.twoPieceBonusDescription = original.twoPieceBonusDescription;
                    instance.threePieceBonusDescription = original.threePieceBonusDescription;

                    // 🔹 Restore saved PamanaData if available
                    if (!string.IsNullOrEmpty(dto.pamanaJson))
                    {
                        var pamanaClone = ScriptableObject.CreateInstance<R_PamanaData>();
                        JsonUtility.FromJsonOverwrite(dto.pamanaJson, pamanaClone);
                        instance.pamanaData = pamanaClone;
                    }
                    else
                    {
                        // fallback for old saves
                        instance.pamanaData = R_PamanaGeneratorUtility.GenerateFrom(instance);
                    }

                    // Reapply visuals
                    var pamanaConfig = Resources.Load<R_PamanaVisualConfig>("Item Database/PamanaVisualConfig");
                    if (pamanaConfig != null)
                    {
                        var visuals = pamanaConfig.GetVisuals(instance.rarity);
                        if (visuals != null)
                        {
                            instance.itemBackdropIcon = visuals.itemBackdropIcon;
                            instance.inventoryHeaderImage = visuals.inventoryHeaderImage;
                            instance.inventoryBackdropImage = visuals.inventoryBackdropImage;
                        }
                    }
                    break;

                case R_ItemType.Agimat:
                    if (original.slot1Ability != null)
                        instance.slot1Ability = Object.Instantiate(original.slot1Ability);
                    if (original.slot2Ability != null)
                        instance.slot2Ability = Object.Instantiate(original.slot2Ability);

                    // 🔹 Tag the Agimat with its saved UID for debugging
                    if (!string.IsNullOrEmpty(dto.uid))
                        instance.name = $"Agimat_{dto.uid}";

                    var agimatConfig = Resources.Load<R_AgimatVisualConfig>("Item Database/AgimatVisualConfig");
                    if (agimatConfig != null)
                    {
                        var set = agimatConfig.GetVisualSet(instance.rarity);
                        if (set != null)
                        {
                            instance.itemBackdropIcon = set.itemBackdropIcon;
                            instance.inventoryHeaderImage = set.inventoryHeaderImage;
                            instance.inventoryBackdropImage = set.inventoryBackdropImage;
                        }
                    }
                    break;

                case R_ItemType.Consumable:
                    instance.consumableEffect = original.consumableEffect;
                    instance.effectText = original.effectText;
                    break;

                case R_ItemType.UpgradeMaterial:
                    instance.upgradeMaterialType = original.upgradeMaterialType;
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[InventorySaver] Could not find item {instance.itemName} in ItemRegistry.");
        }

        return instance;
    }

    static R_InventoryItem FindNextUnassignedInstance(R_Inventory inv, R_ItemData data, HashSet<string> assigned)
    {
        if (inv?.items == null) return null;

        foreach (var s in inv.items)
        {
            if (s == null || s.itemData != data) continue;

            if (string.IsNullOrEmpty(s.runtimeID) || !assigned.Contains(s.runtimeID))
                return s;
        }
        return null;
    }
}
