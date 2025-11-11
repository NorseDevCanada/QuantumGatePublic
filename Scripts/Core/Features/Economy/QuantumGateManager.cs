using System;
using System.Collections.Generic;
using UnityEngine;

public class QuantumGateManager : MonoBehaviour
{
    public static QuantumGateManager Instance;

    [Header("Data")]
    public QuantumGateData gateData;
    public int CurrentGateLevel = 1;

    [Header("XP Rewards")]
    public int BaseXPPerPull = 25;
    public float XPPerLevelFactor = 0.05f;
    public AnimationCurve RarityXPBonusCurve = AnimationCurve.Linear(0, 1, 10, 3f);

    [Header("Gate Unlocking")]
    public int PlayerLevelsPerGate = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // MAIN FUNCTION: Lamp-style Activation
    public List<Gear> DoGateActivation()
    {
        var player = PlayerManager.Instance;
        if (player == null)
        {
            Debug.LogWarning("QuantumGateManager: No PlayerManager found.");
            return null;
        }

        if (!player.SpendQuantumShard())
        {
            Debug.Log("❌ Not enough Quantum Shards to activate gate.");
            return null;
        }

        int pullsThisActivation = GetPullsForLevel(CurrentGateLevel);
        var levelData = gateData != null ? gateData.GetLevel(CurrentGateLevel) : null;
        if (levelData == null)
        {
            Debug.LogWarning("❌ QuantumGateManager: Missing gate data for current level.");
            return null;
        }

        float[] rarityTable = BuildRarityTableForLevel(levelData, CurrentGateLevel);

        List<Gear> results = new List<Gear>();
        for (int i = 0; i < pullsThisActivation; i++)
        {
            int rarityIndex = RollRarity(rarityTable);
            Gear g = CreateGearFromRarity(rarityIndex);
            results.Add(g);

            int xp = CalculateXPRewardPerPull(rarityIndex);
            player.AddXP(xp, XPRewardType.QuantumGate);
        }

        TryAutoUnlockNextGate();

        Debug.Log($"🔮 Gate activated (Lv{CurrentGateLevel}). Pulled {pullsThisActivation} item(s).");
        return results;
    }

    public int GetPullsForLevel(int gateLevel)
    {
        if (gateLevel >= 28) return 20;
        if (gateLevel >= 25) return 15;
        if (gateLevel >= 20) return 10;
        if (gateLevel >= 10) return 5;
        if (gateLevel >= 5) return 3;
        return 1;
    }

    private float[] BuildRarityTableForLevel(QuantumGateLevel levelData, int gateLevel)
    {
        float[] src = levelData.RarityChances;
        float[] dst = new float[src.Length];
        Array.Copy(src, dst, src.Length);

        if (gateLevel >= 20)
        {
            dst[0] = dst[1] = dst[2] = 0f;
        }
        if (gateLevel >= 25)
        {
            dst[3] = 0f;
        }
        if (gateLevel >= 28)
        {
            dst[5] = 0f;
        }

        float sum = 0f;
        foreach (float f in dst) sum += f;
        if (sum <= 0f)
        {
            dst[0] = 1f;
            sum = 1f;
        }
        for (int i = 0; i < dst.Length; i++) dst[i] /= sum;
        return dst;
    }

    private int RollRarity(float[] table)
    {
        float roll = UnityEngine.Random.value;
        float cumulative = 0f;
        for (int i = 0; i < table.Length; i++)
        {
            cumulative += table[i];
            if (roll <= cumulative)
                return i;
        }
        return table.Length - 1;
    }

    private Gear CreateGearFromRarity(int rarityIndex)
    {
        Gear g = ScriptableObject.CreateInstance<Gear>();

        g.Rarity = (GearRarity)Mathf.Clamp(rarityIndex, 0, 10);

        int minLevel = Mathf.Clamp(CurrentGateLevel * 2, 1, 500);
        int maxLevel = Mathf.Clamp(CurrentGateLevel * 3 + 5, minLevel, 600);
        g.GearLevel = UnityEngine.Random.Range(minLevel, maxLevel + 1);

        float rarityMult = GearScaling.GetRarityMultiplier(g.Rarity);

        g.PowerBonus = Mathf.RoundToInt(UnityEngine.Random.Range(5, 25) * rarityMult * (1f + CurrentGateLevel * 0.5f));
        g.HealthBonus = Mathf.RoundToInt(UnityEngine.Random.Range(15, 50) * rarityMult * (1f + CurrentGateLevel * 0.4f));
        g.SpeedBonus = Mathf.RoundToInt(UnityEngine.Random.Range(1, 6) * (rarityMult * 0.75f));

        g.GearName = $"{g.Rarity} Gear +{g.GearLevel}";
        g.CalculateSellValue();

        Debug.Log($"🎁 Pulled {g.Rarity} ({g.GearName}) | Power:{g.PowerBonus} Health:{g.HealthBonus} Speed:{g.SpeedBonus} Sell:{g.SellValue}");

        return g;
    }

    private int CalculateXPRewardPerPull(int rarityIndex)
    {
        float baseXP = BaseXPPerPull * (1f + (CurrentGateLevel * XPPerLevelFactor));
        float rarityBonus = RarityXPBonusCurve.Evaluate(rarityIndex);
        float total = baseXP * rarityBonus;
        return Mathf.RoundToInt(total);
    }

    private void TryAutoUnlockNextGate()
    {
        var player = PlayerManager.Instance;
        if (player == null) return;

        int requiredPlayerLevel = (CurrentGateLevel + 1) * PlayerLevelsPerGate;
        if (CurrentGateLevel < 28 && player.PlayerLevel >= requiredPlayerLevel)
        {
            CurrentGateLevel++;
            Debug.Log($"🌌 Quantum Gate unlocked: Level {CurrentGateLevel}");
        }
    }
}
