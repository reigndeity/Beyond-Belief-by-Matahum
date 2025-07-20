using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatGrowthTable
{
    private static readonly Dictionary<int, float> HPByLevel = new();
    private static readonly Dictionary<int, float> ATKByLevel = new();
    private static readonly Dictionary<int, float> DEFByLevel = new();

    static PlayerStatGrowthTable()
    {
        for (int level = 1; level <= 50; level++)
        {
            float t = (level - 1f) / (50f - 1f); // Normalize [0,1]

            // HP: Exponential-ish
            HPByLevel[level] = Mathf.Lerp(100f, 6411.33f, Mathf.Pow(t, 1.85f));

            // ATK: Slow linear
            ATKByLevel[level] = Mathf.Lerp(15f, 125.22f, t);

            // DEF: Moderate linear
            DEFByLevel[level] = Mathf.Lerp(35f, 402.38f, t);
        }
    }

    public static float GetHP(int level) => HPByLevel.ContainsKey(level) ? HPByLevel[level] : 0f;
    public static float GetATK(int level) => ATKByLevel.ContainsKey(level) ? ATKByLevel[level] : 0f;
    public static float GetDEF(int level) => DEFByLevel.ContainsKey(level) ? DEFByLevel[level] : 0f;
}
