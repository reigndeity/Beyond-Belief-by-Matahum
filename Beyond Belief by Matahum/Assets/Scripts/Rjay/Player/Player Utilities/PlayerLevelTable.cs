using System.Collections.Generic;
using UnityEngine;

public static class PlayerLevelTable
{
    private static readonly Dictionary<int, int> XPToNextLevel = new();

    static PlayerLevelTable()
    {
        for (int level = 1; level < 50; level++)
        {
            XPToNextLevel[level] = Mathf.RoundToInt(100f * Mathf.Pow(level, 1.75f)); // scalable
        }
    }

    public static int GetXPRequiredForLevel(int level)
    {
        return XPToNextLevel.TryGetValue(level, out var xp) ? xp : int.MaxValue;
    }
}
