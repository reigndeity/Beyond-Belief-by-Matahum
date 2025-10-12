using UnityEngine;

[System.Serializable]
public class LootContent
{
    public LootDrops lootPrefab;

    public R_ItemData itemData;
    public int level = 1;
    public bool randomPamana = false;
    public bool randomAgimat = false;
    public bool isGold = false;
    public bool randomGold = false;
    public int goldAmount;
    public Vector2 randomGoldMinMax;
}
