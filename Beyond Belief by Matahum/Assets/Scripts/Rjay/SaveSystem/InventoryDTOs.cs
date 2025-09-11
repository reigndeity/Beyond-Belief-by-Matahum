using System;
using UnityEngine;

namespace SaveSystemDTO
{
    [Serializable]
    public class InventoryDTO
    {
        public InvItemDTO[] items;
    }

    [Serializable]
    public class InvItemDTO
    {
        public string uid;            // matches R_InventoryItem.runtimeID (null for stackables)
        public int count;             // >1 only for stackables
        public string soType;         // AssemblyQualifiedName of the runtime ScriptableObject
        public string soJson;         // full JSON of itemData (runtime clone)
        public string displayName;    // optional, debug

        // NEW: Pamana-specific save
        public string pamanaJson;     // serialized R_PamanaData
    }

    [Serializable]
    public class EquippedDTO
    {
        // Pamana (by slot) — UIDs
        public string pamanaDiwataUid;
        public string pamanaLihimUid;
        public string pamanaSalamangkeroUid;

        // Agimat slots — UIDs
        public string agimatSlot1Uid;
        public string agimatSlot2Uid;
    }
}
