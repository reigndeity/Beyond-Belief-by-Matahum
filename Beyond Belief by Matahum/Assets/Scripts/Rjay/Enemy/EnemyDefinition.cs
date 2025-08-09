using UnityEngine;

public enum GrowthMode { Linear, Exponential }

[CreateAssetMenu(fileName="EnemyDefinition", menuName="Game/Enemies/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("Level Scaling (L1 → L50)")]
    public float hpL1 = 100f, hpL50 = 1000f;
    public float atkL1 = 10f,  atkL50 = 100f;
    public float defL1 = 5f,   defL50 = 50f;

    public GrowthMode growthMode = GrowthMode.Exponential;
    [Range(0.5f, 2f)] public float curvePowHP  = 1f;
    [Range(0.5f, 2f)] public float curvePowATK = 1f;
    [Range(0.5f, 2f)] public float curvePowDEF = 1f;

    [Header("Crit Defaults")]
    [Range(0,100)] public float critRate = 5f;
    [Range(0,500)] public float critDamage = 50f;
}

public static class StatGrowthUtil
{
    public static float Eval(float v1, float v50, int level, float curvePow, GrowthMode mode, int maxLevel=50)
    {
        float t = Mathf.Clamp01((level - 1f) / (maxLevel - 1f));
        if (mode == GrowthMode.Linear) return Mathf.Lerp(v1, v50, t);
        float r = v50 / Mathf.Max(1e-6f, v1);
        float tt = Mathf.Pow(t, Mathf.Max(1e-3f, curvePow));
        return v1 * Mathf.Pow(r, tt);
    }
}
