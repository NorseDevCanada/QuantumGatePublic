using UnityEngine;

public static class CompanionScaling
{
    // Rarity multipliers (inspired by LoM-style power gaps)
    public static float GetRarityMultiplier(CompanionRarity rarity)
    {
        switch (rarity)
        {
            case CompanionRarity.Common:     return 1.0f;
            case CompanionRarity.Rare:       return 1.5f;
            case CompanionRarity.Epic:       return 2.2f;
            case CompanionRarity.Legendary:  return 3.25f;
            case CompanionRarity.Mythic:     return 5.0f;
            default:                         return 1.0f;
        }
    }

    public static float GetLevelPowerMultiplier(int level)
    {
        return 1f + Mathf.Pow(level / 100f, 1.35f);
    }

    public static float GetLevelHealthMultiplier(int level)
    {
        return 1f + Mathf.Pow(level / 100f, 1.5f);
    }
}
