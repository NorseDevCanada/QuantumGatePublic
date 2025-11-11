using UnityEngine;

[System.Serializable]
public class SkillInstance
{
    public SkillData Data;
    public int Level = 1;
    public int CurrentXP = 0;
    public int XPToNextLevel = 100;

    public SkillRarity Rarity => Data != null ? Data.Rarity : SkillRarity.Common;

    // ------------------------------------------------------------------------
    // ⚡ Compute Final Power Value
    // ------------------------------------------------------------------------
    public float GetEffectiveValue()
    {
        if (Data == null) return 0f;

        float rarityMult = SkillScaling.GetRarityMultiplier(Data.Rarity);
        float levelMult = SkillScaling.GetLevelPowerMultiplier(Level);
        float curveMult = Data.LevelGrowth.Evaluate(Level);

        return Data.EffectValue * rarityMult * levelMult * curveMult;
    }

    // ------------------------------------------------------------------------
    // 🧮 Apply Effect (placeholder)
    // ------------------------------------------------------------------------
    public void ApplyEffect(PlayerManager player)
    {
        if (Data == null || player == null) return;

        float value = GetEffectiveValue();

        switch (Data.EffectType)
        {
            case SkillEffectType.FlatPower:
                player.Stats.TotalPower += Mathf.RoundToInt(value);
                break;
            case SkillEffectType.FlatHealth:
                player.Stats.TotalHealth += Mathf.RoundToInt(value);
                break;
            case SkillEffectType.FlatSpeed:
                player.Stats.TotalSpeed += Mathf.RoundToInt(value);
                break;
            case SkillEffectType.PercentPower:
                player.Stats.TotalPower = Mathf.RoundToInt(player.Stats.TotalPower * (1f + value / 100f));
                break;
            case SkillEffectType.PercentHealth:
                player.Stats.TotalHealth = Mathf.RoundToInt(player.Stats.TotalHealth * (1f + value / 100f));
                break;
            case SkillEffectType.PercentCreditsGain:
                player.AddCredits(player.TotalCredits * (value / 100f));
                break;
            case SkillEffectType.PercentXPGain:
                player.AddXP(Mathf.RoundToInt(player.XPToNextLevel * (value / 100f)), XPRewardType.Quest);
                break;
        }
    }

    // ------------------------------------------------------------------------
    // 🎓 Leveling System (200 cap)
    // ------------------------------------------------------------------------
    public void AddXP(int amount)
    {
        if (Data == null) return;

        CurrentXP += amount;

        while (CurrentXP >= XPToNextLevel && Level < RarityCurve.MAX_LEVEL)
        {
            CurrentXP -= XPToNextLevel;
            Level++;

            if (Level >= RarityCurve.MAX_LEVEL)
            {
                Level = RarityCurve.MAX_LEVEL;
                CurrentXP = 0;
                Debug.Log($"✨ {Data.SkillName} reached max level {Level}!");
                break;
            }

            XPToNextLevel = RarityCurve.GetXPRequiredForLevel(Level);
            Debug.Log($"📜 {Data.SkillName} leveled up → Lv {Level} (Next: {XPToNextLevel} XP)");
        }
    }

    // ------------------------------------------------------------------------
    // 🔢 Helper: Display string
    // ------------------------------------------------------------------------
    public string GetDisplayString()
    {
        if (Data == null) return "Unnamed Skill";
        return $"{Data.SkillName} (Lv {Level}) — {Data.EffectType} +{GetEffectiveValue():0.##}";
    }
}
