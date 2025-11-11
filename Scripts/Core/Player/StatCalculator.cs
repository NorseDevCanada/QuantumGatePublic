using System.Collections.Generic;

public static class StatCalculator
{
    public static void ApplyGearBonuses(PlayerStats stats, List<Gear> equippedGear)
    {
        foreach (var gear in equippedGear)
        {
            stats.ATK += gear.GetFinalPower();
            stats.DEF += gear.GetFinalHealth() * 0.5f;
            stats.HP  += gear.GetFinalHealth();
        }
    }

    public static void ApplyBonusesFromStats(PlayerStats stats, Dictionary<StatType, float> bonuses)
    {
        foreach (var kv in bonuses)
        {
            if (stats.ExtraStats.ContainsKey(kv.Key))
                stats.ExtraStats[kv.Key] += kv.Value;
            else
                stats.ExtraStats[kv.Key] = kv.Value;
        }
    }

    public static float ComputeClassScaling(PlayerStats stats, PlayerClassProfile profile)
    {
        float total = 0;
        foreach (var kv in stats.ExtraStats)
        {
            total += kv.Value * profile.GetWeight(kv.Key);
        }
        return total;
    }
}
