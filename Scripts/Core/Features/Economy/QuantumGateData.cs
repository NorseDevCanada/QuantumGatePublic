using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class QuantumGateSegment
{
    public int SegmentNumber;
    public long CreditsRequired;
    public double TimeMinutes;
}

[Serializable]
public class QuantumGateLevel
{
    public int LevelNumber;
    public QuantumGateSegment[] Segments;
    public int MinPlayerLevel = 0;
    public int MinDaysAfterLaunch = 0;
    public float[] RarityChances;
}

[CreateAssetMenu(fileName = "QuantumGateData", menuName = "AFKIdle/QuantumGateData")]
public class QuantumGateData : ScriptableObject
{
    public QuantumGateLevel[] Levels;

    public long baseCredits = 1000;
    public float creditExponent = 1.25f;
    public double baseTimeMinutes = 5.0;
    public float timeExponent = 1.35f;
    public int[] segmentCountsByRange = new int[] { 2, 3, 4, 5, 6 };
    public int[] segmentRangeEnds = new int[] { 5, 10, 14, 21, 28 };

    [Header("Rarity Scaling")]
    [Range(0.1f, 3f)] public float rarityExponent = 1.2f;
    public int eternalUnlockLevel = 28;

    [ContextMenu("Populate Default 28 Levels (Auto)")]
    public void PopulateDefaultLevels()
    {
        Levels = new QuantumGateLevel[28];
        for (int i = 0; i < 28; i++)
        {
            int lvl = i + 1;
            Levels[i] = new QuantumGateLevel();
            Levels[i].LevelNumber = lvl;

            int segCount = 2;
            for (int r = 0; r < segmentRangeEnds.Length; r++)
            {
                if (lvl <= segmentRangeEnds[r])
                {
                    segCount = segmentCountsByRange[r];
                    break;
                }
            }

            Levels[i].Segments = new QuantumGateSegment[segCount];

            double creditsPerSegmentRaw = baseCredits * Math.Pow(lvl, creditExponent);
            long creditsPerSegment;
            try
            {
                creditsPerSegment = (long)Math.Round(creditsPerSegmentRaw);
            }
            catch
            {
                creditsPerSegment = long.MaxValue / segCount;
            }

            double timePerSegment = baseTimeMinutes * Math.Pow(lvl, timeExponent);

            for (int s = 0; s < segCount; s++)
            {
                Levels[i].Segments[s] = new QuantumGateSegment()
                {
                    SegmentNumber = s + 1,
                    CreditsRequired = Math.Max(0, creditsPerSegment),
                    TimeMinutes = Math.Max(0.0, timePerSegment)
                };
            }

            Levels[i].RarityChances = GenerateRarityCurve(lvl);
            Levels[i].MinPlayerLevel = (lvl == 25) ? 50 : (lvl == 28) ? 80 : 0;
            Levels[i].MinDaysAfterLaunch = (lvl == 25) ? 7 : (lvl == 28) ? 14 : 0;
        }

        Debug.Log("QuantumGateData: Populated 28 default levels with rarity scaling.");
    }

    private float[] GenerateRarityCurve(int level)
    {
        float[] baseWeights = new float[] { 1f, 0.6f, 0.45f, 0.3f, 0.2f, 0.15f, 0.1f, 0.07f, 0.05f, 0.03f, 0.02f };

        for (int i = 0; i < baseWeights.Length; i++)
        {
            float rarityTierFactor = Mathf.Pow((float)i / (baseWeights.Length - 1f), rarityExponent);
            baseWeights[i] *= Mathf.Lerp(1f, 4f, (level - 1f) / 27f) * (0.5f + rarityTierFactor);
        }

        if (level < eternalUnlockLevel)
            baseWeights[10] = 0f;

        float sum = baseWeights.Sum();
        if (sum <= 0f) sum = 1f;
        for (int i = 0; i < baseWeights.Length; i++)
            baseWeights[i] /= sum;

        return baseWeights;
    }

    public static readonly string[] RarityNames = new string[]
    {
        "Normal", "Unique", "Well", "Rare", "Mythic", "Epic",
        "Legendary", "Immortal", "Supreme", "Radiant", "Eternal"
    };

    public QuantumGateLevel GetLevel(int levelNumber)
    {
        if (Levels == null || Levels.Length == 0) return null;
        if (levelNumber < 1 || levelNumber > Levels.Length) return null;
        return Levels[levelNumber - 1];
    }

    public string DebugRaritySummary(int level)
    {
        var lvl = GetLevel(level);
        if (lvl == null) return "No data";
        return string.Join(", ", lvl.RarityChances.Select((r, i) =>
            $"{RarityNames[i]} {r * 100f:F1}%"));
    }
}
