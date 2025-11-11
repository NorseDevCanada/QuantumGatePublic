using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompanionRarityTable
{
    public CompanionRarity Rarity;
    [Range(0f, 1f)] public float Chance;
    public CompanionData[] PossibleCompanions;
}

[CreateAssetMenu(fileName = "CompanionSummonData", menuName = "AFKIdle/Companions/SummonData")]
public class CompanionSummonData : ScriptableObject
{
    [Header("Summon Pool")]
    public CompanionRarityTable[] SummonTable;

    public CompanionData GetRandomCompanion()
    {
        float roll = Random.value;
        float cumulative = 0f;

        foreach (var tier in SummonTable)
        {
            cumulative += tier.Chance;
            if (roll <= cumulative && tier.PossibleCompanions.Length > 0)
            {
                int index = Random.Range(0, tier.PossibleCompanions.Length);
                return tier.PossibleCompanions[index];
            }
        }

        // fallback
        return SummonTable.Length > 0 && SummonTable[0].PossibleCompanions.Length > 0
            ? SummonTable[0].PossibleCompanions[0]
            : null;
    }
}
