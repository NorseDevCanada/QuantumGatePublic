// Assets/Scripts/QuantumGate/QuantumGateData.cs
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class QuantumGateSegment
{
    public int SegmentNumber;      // 1..N per level
    public long CreditsRequired;   // credits cost (whole number)
    public double TimeMinutes;     // minutes required for this segment
}

[Serializable]
public class QuantumGateLevel
{
    public int LevelNumber;                     // 1..28
    public QuantumGateSegment[] Segments;       // segments for this level
    public int MinPlayerLevel = 0;              // optional (for special levels)
    public int MinDaysAfterLaunch = 0;          // optional (for special levels)
    public float[] RarityChances;               // length = 11, sum ~= 1.0
}

[CreateAssetMenu(fileName = "QuantumGateData", menuName = "AFKIdle/QuantumGateData")]
public class QuantumGateData : ScriptableObject
{
    public QuantumGateLevel[] Levels; // should be 28 elements

    // ------- Defaults used by the editor helper below (tweak if desired) -------
    public long baseCredits = 1000;       // base credits for level 1
    public float creditExponent = 1.25f;  // scaling exponent for credits
    public double baseTimeMinutes = 5.0;  // base time for level 1 (minutes)
    public float timeExponent = 1.35f;    // time scaling exponent
    public int[] segmentCountsByRange = new int[] { 2, 3, 4, 5, 6 };
    public int[] segmentRangeEnds = new int[] { 5, 10, 14, 21, 28 };

    // Runtime rarity scaling parameters
    [Header("Rarity Scaling")]
    [Tooltip("How strongly higher rarities ramp up per gate level (0–2 typical).")]
    [Range(0.1f, 3f)] public float rarityExponent = 1.2f;
    [Tooltip("Which level unlocks Eternal rarity (default 28).")]
    public int eternalUnlockLevel = 28;

    // -------------------------------------------------------------------------
    #region Populate Defaults
    // -------------------------------------------------------------------------

    [ContextMenu("Populate Default 28 Levels (Auto)")]
    public void PopulateDefaultLevels()
    {
        Levels = new QuantumGateLevel[28];
        for (int i = 0; i < 28; i++)
        {
            int lvl = i + 1;
            Levels[i] = new QuantumGateLevel();
            Levels[i].LevelNumber = lvl;

            // Determine number of segments
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

            // Assign rarity distribution dynamically
            Levels[i].RarityChances = GenerateRarityCurve(lvl);

            // Special restrictions for level 25 and 28
            Levels[i].MinPlayerLevel = (lvl == 25) ? 50 : (lvl == 28) ? 80 : 0;
            Levels[i].MinDaysAfterLaunch = (lvl == 25) ? 7 : (lvl == 28) ? 14 : 0;
        }

        Debug.Log("QuantumGateData: Populated 28 default levels with rarity scaling.");
    }

    #endregion

    // -------------------------------------------------------------------------
    #region Rarity Generation Logic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Generates rarity distribution for a given gate level with smooth scaling.
    /// </summary>
    private float[] GenerateRarityCurve(int level)
    {
        // Order: Normal, Unique, Well, Rare, Mythic, Epic, Legendary, Immortal, Supreme, Radiant, Eternal
        float[] baseWeights = new float[] { 1f, 0.6f, 0.45f, 0.3f, 0.2f, 0.15f, 0.1f, 0.07f, 0.05f, 0.03f, 0.02f };

        // Increase high-rarity weight as level increases
        for (int i = 0; i < baseWeights.Length; i++)
        {
            float rarityTierFactor = Mathf.Pow((float)i / (baseWeights.Length - 1f), rarityExponent);
            baseWeights[i] *= Mathf.Lerp(1f, 4f, (level - 1f) / 27f) * (0.5f + rarityTierFactor);
        }

        // Lock Eternal until the unlock level
        if (level < eternalUnlockLevel)
            baseWeights[10] = 0f;

        // Normalize
        float sum = baseWeights.Sum();
        if (sum <= 0f) sum = 1f;
        for (int i = 0; i < baseWeights.Length; i++)
            baseWeights[i] /= sum;

        return baseWeights;
    }

    /// <summary>
    /// Returns rarity names in the proper order.
    /// </summary>
    public static readonly string[] RarityNames = new string[]
    {
        "Normal", "Unique", "Well", "Rare", "Mythic", "Epic",
        "Legendary", "Immortal", "Supreme", "Radiant", "Eternal"
    };

    /// <summary>
    /// Safe getter for a level (1-based).
    /// </summary>
    public QuantumGateLevel GetLevel(int levelNumber)
    {
        if (Levels == null || Levels.Length == 0) return null;
        if (levelNumber < 1 || levelNumber > Levels.Length) return null;
        return Levels[levelNumber - 1];
    }

    /// <summary>
    /// Helper for previewing rarity distribution in the editor.
    /// </summary>
    public string DebugRaritySummary(int level)
    {
        var lvl = GetLevel(level);
        if (lvl == null) return "No data";
        return string.Join(", ", lvl.RarityChances.Select((r, i) =>
            $"{RarityNames[i]} {r * 100f:F1}%"));
    }
    #endregion
}
