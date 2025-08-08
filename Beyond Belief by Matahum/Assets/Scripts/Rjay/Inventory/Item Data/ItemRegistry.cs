using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ItemRegistry
{
    static Dictionary<string, R_ItemData> _byId;
    static Dictionary<string, R_ItemData> _byAssetName;
    static Dictionary<string, R_ItemData> _byItemName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        _byId = new();
        _byAssetName = new();
        _byItemName = new();

        // Load ALL R_ItemData under any Resources folder
        var all = Resources.LoadAll<R_ItemData>(""); 
        foreach (var it in all)
        {
            if (it == null) continue;

            var id = GetStableId(it);                      // ItemId / asset name / itemName
            var assetName = it.name;
            var itemName = TryGetItemNameField(it);

            if (!string.IsNullOrEmpty(id) && !_byId.ContainsKey(id))           _byId.Add(id, it);
            if (!string.IsNullOrEmpty(assetName) && !_byAssetName.ContainsKey(assetName)) _byAssetName.Add(assetName, it);
            if (!string.IsNullOrEmpty(itemName) && !_byItemName.ContainsKey(itemName))    _byItemName.Add(itemName, it);
        }
    }

    public static string GetStableId(R_ItemData data)
    {
        // Prefer a real ItemId if you add it later
        var t = data.GetType();
        var prop = t.GetProperty("ItemId", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(string))
        {
            var v = prop.GetValue(data) as string;
            if (!string.IsNullOrEmpty(v)) return v;
        }

        var field = t.GetField("itemId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(string))
        {
            var v = field.GetValue(data) as string;
            if (!string.IsNullOrEmpty(v)) return v;
        }

        // Fallbacks (less ideal—avoid renaming assets if you rely on this)
        var itemName = TryGetItemNameField(data);
        if (!string.IsNullOrEmpty(itemName)) return itemName;

        return data.name; // asset name
    }

    public static bool TryGetByAnyId(string anyId, out R_ItemData item)
    {
        if (string.IsNullOrEmpty(anyId)) { item = null; return false; }
        return _byId.TryGetValue(anyId, out item)
            || _byAssetName.TryGetValue(anyId, out item)
            || _byItemName.TryGetValue(anyId, out item);
    }

    public static bool MatchesId(R_ItemData data, string anyId)
    {
        if (data == null || string.IsNullOrEmpty(anyId)) return false;
        if (GetStableId(data) == anyId) return true;
        if (data.name == anyId) return true;
        return TryGetItemNameField(data) == anyId;
    }

    static string TryGetItemNameField(R_ItemData data)
    {
        var t = data.GetType();
        var field = t.GetField("itemName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(string))
            return field.GetValue(data) as string;
        return null;
    }
}
