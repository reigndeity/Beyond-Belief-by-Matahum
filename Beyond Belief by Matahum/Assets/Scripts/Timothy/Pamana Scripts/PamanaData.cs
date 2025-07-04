using System;
using UnityEngine;

[System.Serializable]
public class PamanaData
{
    public Sprite icon;
    public string uniqueID;
    public string pamanaName;
    public string type;
    public string rarity;
    public int pamanaLevel;
    public SetPieceBonus setPiece;
    public PamanaStat mainStat;
    public PamanaStat subStat;
    public float randomMainValue;
    public float randomSubValue;

    public PamanaData(Pamana pamana)
    {
        icon = pamana.icon;
        uniqueID = pamana.uniqueID; // ✅ Save the unique ID
        pamanaName = pamana.pamanaName;
        type = pamana.type.ToString();
        rarity = pamana.rarity.ToString();
        pamanaLevel = pamana.pamanaLevel;
        setPiece = pamana.setPiece;
        mainStat = pamana.mainStat;
        subStat = pamana.subStat;
        randomMainValue = pamana.randomMainValue;
        randomSubValue = pamana.randomSubValue;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static PamanaData FromJson(string json)
    {
        return JsonUtility.FromJson<PamanaData>(json);
    }
}


