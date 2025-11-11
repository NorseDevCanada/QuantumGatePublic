using UnityEngine;

public static class RarityCurve
{
    public const int MAX_LEVEL = 200;

    public static int GetXPRequiredForLevel(int level)
    {
        level = Mathf.Clamp(level, 1, MAX_LEVEL);
        float xp = 100f * Mathf.Pow(level, 2.05f);
        return Mathf.RoundToInt(xp);
    }

    public static int GetDupeXP(CompanionRarity rarity, int currentLevel)
    {
        float rarityMult = rarity switch
        {
            CompanionRarity.Common => 0.35f,
            CompanionRarity.Rare => 0.55f,
            CompanionRarity.Epic => 0.8f,
            CompanionRarity.Legendary => 1.0f,
            CompanionRarity.Mythic => 1.25f,
            _ => 0.35f
        };

        int needed = GetXPRequiredForLevel(Mathf.Max(1, currentLevel));
        return Mathf.RoundToInt(needed * rarityMult);
    }

    public static int GetDupeXP(SkillRarity rarity, int currentLevel)
    {
        float rarityMult = rarity switch
        {
            SkillRarity.Common => 0.35f,
            SkillRarity.Rare => 0.55f,
            SkillRarity.Epic => 0.8f,
            SkillRarity.Legendary => 1.0f,
            SkillRarity.Mythic => 1.25f,
            _ => 0.35f
        };

        int needed = GetXPRequiredForLevel(Mathf.Max(1, currentLevel));
        return Mathf.RoundToInt(needed * rarityMult);
    }
}
