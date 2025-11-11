// Scripts/Combat/GearScaling.cs
using UnityEngine;

public static class GearScaling
{
    // Rarity multipliers tuned like idle/gacha games (LoM-style curve)
    private static readonly float[] rarityMultipliers = new float[]
    {
        1.00f,  // Normal
        1.10f,  // Unique
        1.20f,  // Well
        1.35f,  // Rare
        1.55f,  // Mythic
        1.80f,  // Epic
        2.10f,  // Legendary
        2.50f,  // Immortal
        3.00f,  // Supreme
        3.60f,  // Radiant
        4.50f   // Eternal
    };

    /// <summary>
    /// Returns a multiplier based on rarity index.
    /// </summary>
    public static float GetRarityMultiplier(GearRarity rarity)
    {
        int idx = (int)rarity;
        if (idx < 0 || idx >= rarityMultipliers.Length)
            return 1f;
        return rarityMultipliers[idx];
    }

    /// <summary>
    /// Legend-of-Mushroom style: gear level matters a lot.
    /// We let level scale slightly superlinear.
    /// </summary>
    public static float GetLevelMultiplier(int gearLevel)
    {
        // level^1.05 gives nice idle-ish scaling
        return Mathf.Pow(Mathf.Max(1, gearLevel), 1.05f);
    }

    /// <summary>
    /// Final stat bonus = (baseStatFromSO) * rarityMult * levelMult
    /// </summary>
    public static int GetFinalStat(int baseValue, GearRarity rarity, int gearLevel)
    {
        float r = GetRarityMultiplier(rarity);
        float l = GetLevelMultiplier(gearLevel);
        return Mathf.RoundToInt(baseValue * r * l);
    }
}
