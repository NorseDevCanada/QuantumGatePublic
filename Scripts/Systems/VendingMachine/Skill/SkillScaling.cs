using UnityEngine;

public static class SkillScaling
{
    // ------------------------------------------------------------------------
    // 🎯 Base rarity multipliers — tuned to mirror CompanionScaling
    // ------------------------------------------------------------------------
    // Used for both damage and effectiveness scaling.
    public static float GetRarityMultiplier(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common:     return 1.0f;
            case SkillRarity.Rare:       return 1.4f;
            case SkillRarity.Epic:       return 2.0f;
            case SkillRarity.Legendary:  return 3.0f;
            case SkillRarity.Mythic:     return 4.5f;
            default:                     return 1.0f;
        }
    }

    // ------------------------------------------------------------------------
    // ⚡ Per-level multipliers — exponential scaling (LoM-style)
    // ------------------------------------------------------------------------
    // These affect power, cooldown efficiency, or any numerical stat.
    public static float GetLevelPowerMultiplier(int level)
    {
        // Small curve bump that becomes noticeable after Lv100+
        return 1f + Mathf.Pow(level / 100f, 1.25f);
    }

    public static float GetLevelEffectMultiplier(int level)
    {
        // Useful for buff/debuff potency, %damage, or duration tuning
        return 1f + Mathf.Pow(level / 120f, 1.35f);
    }

    // ------------------------------------------------------------------------
    // 🧮 Unified Skill Strength Calculator
    // ------------------------------------------------------------------------
    public static int CalculateSkillPower(int baseValue, SkillRarity rarity, int level)
    {
        float rarityMult = GetRarityMultiplier(rarity);
        float levelMult = GetLevelPowerMultiplier(level);
        return Mathf.RoundToInt(baseValue * rarityMult * levelMult);
    }
}
