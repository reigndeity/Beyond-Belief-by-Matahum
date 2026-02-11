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

    public LootContent Clone()
    {
        return new LootContent
        {
            lootPrefab = this.lootPrefab,
            itemData = this.itemData,
            level = this.level,
            randomPamana = this.randomPamana,
            randomAgimat = this.randomAgimat,
            isGold = this.isGold,
            randomGold = this.randomGold,
            goldAmount = this.goldAmount,
            randomGoldMinMax = this.randomGoldMinMax
        };
    }
}
