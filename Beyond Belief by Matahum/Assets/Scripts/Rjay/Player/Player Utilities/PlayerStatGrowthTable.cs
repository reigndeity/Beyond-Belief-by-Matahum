using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatGrowthTable
{
    private const int MAX_LEVEL = 50;

    // Endpoints (Level 1 → Level 50) — from your spec
    private const float HP_L1 = 200f, HP_L50 = 5766.03f; //hplevel original 911.79
    private const float ATK_L1 = 17.81f, ATK_L50 = 112.62f;
    private const float DEF_L1 = 57.23f, DEF_L50 = 361.88f;

    public enum GrowthMode { Linear, Exponential }

    // Pick your mode here:
    public static GrowthMode Mode = GrowthMode.Exponential; // or GrowthMode.Linear

    // Exponential shaping: t^k. Keep 1 for pure geometric; >1 makes late levels ramp harder.
    public static float HpCurve = 1f, AtkCurve = 1f, DefCurve = 1f;

    private static readonly Dictionary<int, float> HPByLevel = new();
    private static readonly Dictionary<int, float> ATKByLevel = new();
    private static readonly Dictionary<int, float> DEFByLevel = new();

    static PlayerStatGrowthTable() { Rebuild(); }

    public static void Rebuild()
    {
        HPByLevel.Clear(); ATKByLevel.Clear(); DEFByLevel.Clear();

        for (int level = 1; level <= MAX_LEVEL; level++)
        {
            float t = (level - 1f) / (MAX_LEVEL - 1f);
            HPByLevel[level]  = Interp(HP_L1,  HP_L50,  t, Mode, HpCurve);
            ATKByLevel[level] = Interp(ATK_L1, ATK_L50, t, Mode, AtkCurve);
            DEFByLevel[level] = Interp(DEF_L1, DEF_L50, t, Mode, DefCurve);
        }
    }

    private static float Interp(float v1, float v50, float t, GrowthMode mode, float curvePow)
    {
        if (mode == GrowthMode.Linear) return Mathf.Lerp(v1, v50, t);

        // Exponential (geometric) — matches endpoints exactly; curvePow shapes pacing
        float r = v50 / v1;
        float tt = curvePow <= 0f ? t : Mathf.Pow(t, curvePow);
        return v1 * Mathf.Pow(r, tt);
    }

    private static int ClampLvl(int level) => Mathf.Clamp(level, 1, MAX_LEVEL);
    public static float GetHP(int level)  => HPByLevel[ClampLvl(level)];
    public static float GetATK(int level) => ATKByLevel[ClampLvl(level)];
    public static float GetDEF(int level) => DEFByLevel[ClampLvl(level)];
}
