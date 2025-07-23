using UnityEngine;

public static class WeaponStatGrowthTable
{
    public static float GetATK(int level)
    {
        level = Mathf.Clamp(level, 1, 50);
        float startATK = 44f;
        float endATK = 319f;
        float t = (level - 1f) / 49f; // Normalize from 0 to 1
        return Mathf.Lerp(startATK, endATK, t);
    }
}

