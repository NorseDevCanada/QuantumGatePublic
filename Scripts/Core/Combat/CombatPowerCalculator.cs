using UnityEngine;

public static class CombatPowerCalculator
{
    // Base weighting for how much each stat contributes to overall Combat Power
    private const float PowerWeight = 1.25f;
    private const float HealthWeight = 0.75f;
    private const float SpeedWeight = 0.5f;

    // Master function for CP computation
    public static int CalculateCombatPower(PlayerStats stats, PlayerClassType classType)
    {
        float baseCP =
            (stats.TotalPower * PowerWeight) +
            (stats.TotalHealth * HealthWeight) +
            (stats.TotalSpeed * SpeedWeight);

        float classMultiplier = GetClassMultiplier(stats, classType);
        return Mathf.RoundToInt(baseCP * classMultiplier);
    }

    // ----------------------------------------------------------------------
    // 💥 Class-specific scaling logic
    // ----------------------------------------------------------------------
    private static float GetClassMultiplier(PlayerStats stats, PlayerClassType classType)
    {
        switch (classType)
        {
            case PlayerClassType.PistolSpecialist:
                // Agile and combo-heavy — scales better with Speed
                return 1.0f + (stats.TotalSpeed * 0.0012f) + (stats.TotalPower * 0.0006f);

            case PlayerClassType.RifleOperative:
                // Balanced ranged fighter — Power and Speed synergy
                return 1.0f + (stats.TotalPower * 0.0010f) + (stats.TotalSpeed * 0.0008f);

            case PlayerClassType.MeleeSpecialist:
                // Defensive and close-combat — Health and Power synergy
                return 1.0f + (stats.TotalHealth * 0.0009f) + (stats.TotalPower * 0.0007f);

            default:
                return 1.0f;
        }
    }
}
