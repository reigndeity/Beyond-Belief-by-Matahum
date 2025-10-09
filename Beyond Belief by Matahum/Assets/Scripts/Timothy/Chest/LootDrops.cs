using UnityEngine;

public class LootDrops : Interactable
{
    [HideInInspector] public ChestContent lootContent;
    private R_Inventory inventory;
    private int goldToAdd;

    private void Start()
    {
        inventory = FindFirstObjectByType<R_Inventory>();
        Destroy(gameObject, 30f);
    }

    public void Initialize()
    {
        RandomAgimatOrPamanaOrGold();

        if (lootContent.itemData != null)
        {
            interactName = lootContent.itemData.itemName;
            icon = lootContent.itemData.itemIcon;
        }
    }

    public override void OnInteract()
    {
        ClaimItem();
    }

    void ClaimItem()
    {
        if(lootContent.itemData != null)
        {
            SpecificItem();
        }
        else if(lootContent.isGold) //Gold
        {
            Player player = FindFirstObjectByType<Player>();
            player.AddGoldCoins(goldToAdd);
        }

        Destroy(gameObject);
    }

    void SpecificItem()
    {
        if (lootContent.itemData.itemType != R_ItemType.Agimat ||
                lootContent.itemData.itemType != R_ItemType.Pamana)
        {
            inventory.AddItem(lootContent.itemData, 1);
        }
        else if (lootContent.itemData.itemType == R_ItemType.Pamana)
        {
            R_GeneralItemSpawner.instance.SpawnSinglePamana(new R_ItemData[] { lootContent.itemData });
        }
        else if (lootContent.itemData.itemType == R_ItemType.Agimat)
        {
            R_GeneralItemSpawner.instance.SpawnSingleAgimat(new R_ItemData[] { lootContent.itemData });
        }
    }

    public void RandomAgimatOrPamanaOrGold()
    {
        if (lootContent.randomAgimat)
        {
            int randomizer = Random.Range(0, R_GeneralItemSpawner.instance.pamanaTemplates.Length);
            R_ItemData itemData = R_GeneralItemSpawner.instance.pamanaTemplates[randomizer];
            lootContent.itemData = itemData;
        }
        else if (lootContent.randomPamana)
        {
            int randomizer = Random.Range(0, R_GeneralItemSpawner.instance.agimatTemplates.Length);
            R_ItemData itemData = R_GeneralItemSpawner.instance.agimatTemplates[randomizer];
            lootContent.itemData = itemData;
        }
        else if (lootContent.isGold)
        {
            goldToAdd = GoldGenerator();
            interactName = $"{goldToAdd} Coins";
        }
    }

    int GoldGenerator()
    {
        int addGold;

        if (lootContent.randomGold)
        {
            int min = (int)lootContent.randomGoldMinMax.x;
            int max = (int)lootContent.randomGoldMinMax.y;
            addGold = Random.Range(min, max);
        }
        else
        {
            addGold = lootContent.goldAmount;
        }

        return addGold;
    }
}
