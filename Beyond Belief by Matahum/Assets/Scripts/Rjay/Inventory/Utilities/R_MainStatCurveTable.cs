
using System.Collections.Generic;
using UnityEngine;

public static class R_MainStatCurveTable
{
    public static readonly Dictionary<R_StatType, float[]> MainStatCurves = new()
    {
        { R_StatType.FlatHP, new float[] {
            717, 920, 1123, 1326, 1530, 1733, 1936, 2139, 2342, 2545,
            2749, 2952, 3155, 3358, 3561, 3764, 3967, 4171, 4374, 4577, 4780
        }},
        { R_StatType.FlatATK, new float[] {
            47, 60, 73, 86, 100, 113, 126, 139, 152, 166,
            179, 192, 205, 219, 232, 245, 258, 272, 285, 298, 311
        }},
        { R_StatType.PercentHP, new float[] {
            7.0f, 9.0f, 11.0f, 12.9f, 14.9f, 16.9f, 18.9f, 20.9f, 22.8f, 24.8f,
            26.8f, 28.8f, 30.8f, 32.8f, 34.7f, 36.7f, 38.7f, 40.7f, 42.7f, 44.6f, 46.6f
        }},
        { R_StatType.PercentATK, new float[] {
            7.0f, 9.0f, 11.0f, 12.9f, 14.9f, 16.9f, 18.9f, 20.9f, 22.8f, 24.8f,
            26.8f, 28.8f, 30.8f, 32.8f, 34.7f, 36.7f, 38.7f, 40.7f, 42.7f, 44.6f, 46.6f
        }},
        { R_StatType.PercentDEF, new float[] {
            8.7f, 11.2f, 13.7f, 16.2f, 18.6f, 21.1f, 23.6f, 26.1f, 28.6f, 31.0f,
            33.5f, 36.0f, 38.5f, 40.9f, 43.4f, 45.9f, 48.4f, 50.8f, 53.3f, 55.8f, 58.3f
        }},
        { R_StatType.CRITRate, new float[] {
            4.7f, 6.0f, 7.3f, 8.6f, 9.9f, 11.3f, 12.6f, 13.9f, 15.2f, 16.6f,
            17.9f, 19.2f, 20.5f, 21.8f, 23.2f, 24.5f, 25.8f, 27.1f, 28.4f, 29.8f, 31.1f
        }},
        { R_StatType.CRITDamage, new float[] {
            9.3f, 12.0f, 14.6f, 17.3f, 19.9f, 22.5f, 25.2f, 27.8f, 30.5f, 33.1f,
            35.7f, 38.4f, 41.0f, 43.7f, 46.3f, 49.0f, 51.6f, 54.2f, 56.9f, 59.5f, 62.2f
        }},
        { R_StatType.CooldownReduction, new float[] {
            4.7f, 6.0f, 7.3f, 8.6f, 9.9f, 11.3f, 12.6f, 13.9f, 15.2f, 16.6f,
            17.9f, 19.2f, 20.5f, 21.8f, 23.2f, 24.5f, 25.8f, 27.1f, 28.4f, 29.8f, 31.1f
        }},
    };

    public static float GetMainStatValue(R_StatType statType, int level)
    {
        if (!MainStatCurves.ContainsKey(statType))
        {
            Debug.LogWarning($"No main stat curve defined for: {statType}");
            return 0f;
        }

        float[] curve = MainStatCurves[statType];
        level = Mathf.Clamp(level, 0, curve.Length - 1);
        return curve[level];
    }
}
