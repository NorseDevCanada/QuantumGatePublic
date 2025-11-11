using UnityEngine;

[System.Serializable]
public class CompanionInstance
{
    public CompanionData Data;
    public int Level = 1;
    public int CurrentXP = 0;
    public int XPToNextLevel = 100;

    public CompanionRarity Rarity => Data != null ? Data.Rarity : CompanionRarity.Common;

    public int GetPower()
    {
        if (Data == null) return 0;
        float rarityMult = CompanionScaling.GetRarityMultiplier(Data.Rarity);
        float powerScale = Data.PowerGrowth.Evaluate(Level);
        return Mathf.RoundToInt(Data.BasePower * rarityMult * powerScale);
    }

    public int GetHealth()
    {
        if (Data == null) return 0;
        float rarityMult = CompanionScaling.GetRarityMultiplier(Data.Rarity);
        float healthScale = Data.HealthGrowth.Evaluate(Level);
        return Mathf.RoundToInt(Data.BaseHealth * rarityMult * healthScale);
    }

    public int GetCombatPower()
    {
        // same CP logic as before
        return Mathf.RoundToInt(GetPower() * 2f + GetHealth() * 0.5f);
    }

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
                Debug.Log($"🐾 {Data.CompanionName} reached max level {Level}!");
                break;
            }

            XPToNextLevel = RarityCurve.GetXPRequiredForLevel(Level);
            Debug.Log($"🐾 {Data.CompanionName} leveled up → Lv {Level} (Next: {XPToNextLevel} XP)");
        }
    }
}
