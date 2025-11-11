using UnityEngine;

public enum GearRarity
{
    Normal,
    Unique,
    Well,
    Rare,
    Mythic,
    Epic,
    Legendary,
    Immortal,
    Supreme,
    Radiant,
    Eternal
}

[CreateAssetMenu(fileName = "NewGear", menuName = "AFKIdle/Gear")]
public class Gear : ScriptableObject
{
    [Header("Base Info")]
    public string GearName;
    public GearRarity Rarity = GearRarity.Normal;

    [Header("Stats")]
    public int GearLevel = 1;
    public int PowerBonus;
    public int HealthBonus;
    public int SpeedBonus;

    [Header("Economy")]
    public int SellValue;

    // Compatibility property for old scripts referencing "Name"
    public string Name => GearName;

    // ------------------------------------------------------------------------
    // ⚙️ Scaled Stat Accessors (LoM-style)
    // ------------------------------------------------------------------------
    public int GetFinalPower()
    {
        return GearScaling.GetFinalStat(PowerBonus, Rarity, GearLevel);
    }

    public int GetFinalHealth()
    {
        return GearScaling.GetFinalStat(HealthBonus, Rarity, GearLevel);
    }

    public int GetFinalSpeed()
    {
        return GearScaling.GetFinalStat(SpeedBonus, Rarity, GearLevel);
    }

    // ------------------------------------------------------------------------
    // 💰 Economy Helpers
    // ------------------------------------------------------------------------
    public void CalculateSellValue()
    {
        // Sell value formula inspired by idle gachas: 
        // stronger + rarer = higher sell
        float rarityMult = GearScaling.GetRarityMultiplier(Rarity);
        float levelMult = GearScaling.GetLevelMultiplier(GearLevel);
        float baseWorth = PowerBonus * 2f + HealthBonus * 0.5f + SpeedBonus * 3f;

        SellValue = Mathf.RoundToInt(baseWorth * rarityMult * levelMult * 0.25f);
    }
}
