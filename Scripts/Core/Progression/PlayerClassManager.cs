using UnityEngine;
using System;

/// <summary>
/// Defines the 3 core playable classes.
/// </summary>
public enum PlayerClassType { PistolSpecialist, RifleOperative, MeleeSpecialist }

/// <summary>
/// Defines a tier upgrade (unlocked by player level + gate progress).
/// </summary>
[System.Serializable]
public class ClassUpgrade
{
    public string UpgradeName;
    public int PlayerLevelRequirement;
    public int QuantumGateLevelRequirement;
    [TextArea]
    public string BonusDescription;

    // Optional backend hook — defines a flat stat or multiplier bonus
    public StatType AffectedStat;
    public float BonusValue;
}

/// <summary>
/// Container for one class and its 4 upgrades.
/// </summary>
[System.Serializable]
public class PlayerClass
{
    public PlayerClassType ClassType;
    public string ClassName;
    public ClassUpgrade Starter;
    public ClassUpgrade[] Upgrades = new ClassUpgrade[4];
}

/// <summary>
/// ScriptableObject container for all available classes.
/// </summary>
[CreateAssetMenu(fileName = "PlayerClassData", menuName = "AFKIdle/PlayerClassData")]
public class PlayerClassData : ScriptableObject
{
    public PlayerClass[] Classes;
}

/// <summary>
/// Runtime class manager — determines active upgrade tier and applies bonuses.
/// </summary>
public class PlayerClassManager : MonoBehaviour
{
    public static PlayerClassManager Instance;
    public PlayerClassData ClassData;

    [Header("Runtime State")]
    public PlayerClassType CurrentClass;
    public int PlayerLevel;
    public int QuantumGateLevel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Returns the current upgrade the player qualifies for.
    /// </summary>
    public ClassUpgrade GetActiveUpgrade()
    {
        PlayerClass pClass = Array.Find(ClassData.Classes, c => c.ClassType == CurrentClass);
        if (pClass == null) return null;

        ClassUpgrade active = pClass.Starter;
        foreach (ClassUpgrade upgrade in pClass.Upgrades)
        {
            if (PlayerLevel >= upgrade.PlayerLevelRequirement &&
                QuantumGateLevel >= upgrade.QuantumGateLevelRequirement)
            {
                active = upgrade;
            }
            else break;
        }
        return active;
    }

    /// <summary>
    /// Applies the current class's bonuses to the player's stats.
    /// </summary>
    public void ApplyClassBonuses(PlayerStats stats)
    {
        var upgrade = GetActiveUpgrade();
        if (upgrade == null) return;

        // Apply the base upgrade bonus (backend scaling)
        if (upgrade.BonusValue > 0 && upgrade.AffectedStat != StatType.HP)
        {
            if (!stats.ExtraStats.ContainsKey(upgrade.AffectedStat))
                stats.ExtraStats[upgrade.AffectedStat] = 0;

            stats.ExtraStats[upgrade.AffectedStat] += upgrade.BonusValue;
        }

        Debug.Log($"✅ Applied class bonus: {upgrade.UpgradeName} ({upgrade.BonusDescription})");
    }

    /// <summary>
    /// Debug helper to log the current unlocked class upgrade.
    /// </summary>
    public void PrintCurrentUpgrade()
    {
        var upgrade = GetActiveUpgrade();
        if (upgrade == null) return;
        Debug.Log($"Current Upgrade: {upgrade.UpgradeName} - {upgrade.BonusDescription}");
    }
}
