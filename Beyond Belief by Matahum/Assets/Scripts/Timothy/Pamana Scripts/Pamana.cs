using UnityEngine;
using System.IO;
public enum PamanaRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum PamanaType
{
    Diwata,
    Lihim_ng_Karunungan,
    Salamangkero
}

public enum SetPiece
{
    Katiwala_ng_Araw,
    Hinagpis_ng_Digmaan,
    Katiwala_ng_Buwan,
    Malabulkang_Maniniil,
    Tagapangasiwa_ng_Layog,
    Soberano_ng_Unos,
    Yakap_ng_Ilog,
    Bulong_sa_Kagubatan,
    Propeta_ng_Kamatayan
}


public enum StatType
{
    CritRate,
    CritDmg,
    AtkPrcnt,
    FlatAtk,
    HpPrcnt,
    FlatHp,
    DefPrcnt,
    FlatDef
}


[System.Serializable]
public class PamanaStat
{
    public StatType statType;
    public float value;
}

[System.Serializable]
public class SetPieceBonus
{
    public SetPiece setPieceName;
    public PamanaStat firstSetBonus;
    public PamanaStat secondSetBonus;
}

[CreateAssetMenu(fileName = "New Pamana", menuName = "Pamana System/Pamana")]
public class Pamana : ScriptableObject
{
    [Header("Pamana Statistics")]
    public Sprite icon;
    public string uniqueID;
    public PamanaRarity rarity;
    public int pamanaLevel = 1;
    public string pamanaName;
    public PamanaType type;
    public SetPieceBonus setPiece;
    public PamanaStat mainStat;
    public PamanaStat subStat;
    public float randomMainValue;
    public float randomSubValue;

    private PlayerStats player;
    private string savePath;
    private int maxLevel;

    private void OnEnable()
    {
        savePath = savePath = Application.persistentDataPath + "/GeneralItems";
        maxLevel = GetMaxLevel();
    }

    public Pamana CreateInstance()
    {
        // Clone the ScriptableObject to avoid modifying the original asset
        Pamana newInstance = Instantiate(this);
        newInstance.uniqueID = System.Guid.NewGuid().ToString(); // ✅ Assign a new unique ID to the clone
        Debug.Log($"Generated Pamana Unique ID: {newInstance.uniqueID}");
        newInstance.DetermineRarity(); // Set rarity based on player level
        newInstance.GenerateRandomStats(); // Generate stats based on rarity

        return newInstance;
    }


    public void DetermineRarity()
    {
        player = GameObject.FindFirstObjectByType<PlayerStats>();
        if (player == null)
        {
            Debug.LogError("PlayerStats reference is missing!");
            return;
        }

        float roll = Random.value; // Random number between 0 and 1

        // if (player.p_level <= 10) // Tier 1
        // {
        //     if (roll < 0.80f) rarity = PamanaRarity.Common;
        //     else rarity = PamanaRarity.Rare;
        // }
        // else if (player.p_level <= 20) // Tier 2
        // {
        //     if (roll < 0.30f) rarity = PamanaRarity.Common;
        //     else if (roll < 0.95f) rarity = PamanaRarity.Rare;
        //     else rarity = PamanaRarity.Epic;
        // }
        // else if (player.p_level <= 30) // Tier 3
        // {
        //     if (roll < 0.85f) rarity = PamanaRarity.Rare;
        //     else rarity = PamanaRarity.Epic;
        // }
        // else if (player.p_level <= 40) // Tier 4
        // {
        //     if (roll < 0.90f) rarity = PamanaRarity.Epic;
        //     else rarity = PamanaRarity.Legendary;
        // }
        // else if (player.p_level <= 50) // Tier 5
        // {
        //     if (roll < 0.60f) rarity = PamanaRarity.Epic;
        //     else rarity = PamanaRarity.Legendary;
        // }

        AssignStatRange();
    }

    void AssignStatRange()
    {
        switch (rarity)
        {
            case PamanaRarity.Common:
                randomMainValue = Random.Range(0.6f, 1.8f);
                randomSubValue = Random.Range(0.6f, 1.2f);
                break;

            case PamanaRarity.Rare:
                randomMainValue = Random.Range(0.7f, 2.1f);
                randomSubValue = Random.Range(0.7f, 1.4f);
                break;

            case PamanaRarity.Epic:
                randomMainValue = Random.Range(0.8f, 2.4f);
                randomSubValue = Random.Range(0.8f, 1.6f);
                break;

            case PamanaRarity.Legendary:
                randomMainValue = Random.Range(1f, 3f);
                randomSubValue = Random.Range(1f, 2f);
                break;
        }
    }

    public void GenerateRandomStats()
    {
        StatType[] mainStatOptions = null;
        StatType[] subStatOptions = null;

        // Define possible stats per type
        if (type == PamanaType.Diwata)
        {
            mainStatOptions = new StatType[] { StatType.CritRate, StatType.CritDmg, StatType.AtkPrcnt };
            subStatOptions = new StatType[] { StatType.FlatAtk, StatType.HpPrcnt, StatType.DefPrcnt };
        }
        else if (type == PamanaType.Lihim_ng_Karunungan)
        {
            mainStatOptions = new StatType[] { /*StatType.HpRegen,*/ StatType.DefPrcnt, StatType.FlatDef };
            subStatOptions = new StatType[] { StatType.HpPrcnt, StatType.FlatHp /*movespeed*/ };
        }
        else if (type == PamanaType.Salamangkero)
        {
            mainStatOptions = new StatType[] { StatType.AtkPrcnt, StatType.FlatAtk, StatType.CritDmg };
            subStatOptions = new StatType[] { StatType.CritRate, StatType.HpPrcnt, StatType.FlatDef };
        }

        // Ensure arrays are assigned before accessing them
        if (mainStatOptions != null && subStatOptions != null)
        {
            mainStat = new PamanaStat
            {
                statType = mainStatOptions[Random.Range(0, mainStatOptions.Length)],
                value = randomMainValue
            };

            subStat = new PamanaStat
            {
                statType = subStatOptions[Random.Range(0, subStatOptions.Length)],
                value = randomSubValue
            };
        }
        else
        {
            Debug.LogError("PamanaType is not set correctly!");
        }
    }

    public int GetMaxLevel()
    {
        switch (rarity)
        {
            case PamanaRarity.Common: return 5;
            case PamanaRarity.Rare: return 10;
            case PamanaRarity.Epic: return 15;
            case PamanaRarity.Legendary: return 20;
            default: return 5;
        }
    }

    public void LevelUp()
    {
        if (pamanaLevel < maxLevel)
        {
            pamanaLevel++;
            PlayerPrefs.SetInt($"{uniqueID}'s Level", pamanaLevel);
            mainStat.value += randomMainValue;
            subStat.value += randomSubValue;
            SavePamanaJson();
        }
        else
        {
            Debug.Log("Pamana has reached max level!");
        }
    }

    public void SavePamanaJson()
    {
        PamanaData data = new PamanaData(this);
        string json = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(savePath, uniqueID + ".json");
        File.WriteAllText(filePath, json);
        Debug.Log("Pamana saved: " + filePath);
    }
}
