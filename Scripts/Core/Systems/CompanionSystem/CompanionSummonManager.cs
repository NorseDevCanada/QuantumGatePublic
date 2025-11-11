using System.Collections.Generic;
using UnityEngine;

public class CompanionSummonManager : MonoBehaviour
{
    public static CompanionSummonManager Instance;

    [Header("Summon Data")]
    public List<CompanionData> CompanionPool = new List<CompanionData>();

    [Header("Summon Costs")]
    public int CostFor15 = 15;
    public int CostFor35 = 30;

    [Header("Rarity Chances (sum = 1.0)")]
    public float CommonChance = 0.55f;
    public float RareChance = 0.25f;
    public float EpicChance = 0.12f;
    public float LegendaryChance = 0.06f;
    public float MythicChance = 0.02f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------------------------------------------------------------
    // 🎰 Summon Logic
    // ------------------------------------------------------------------------
    public List<CompanionInstance> Summon15()
    {
        return TrySummon(15, CostFor15);
    }

    public List<CompanionInstance> Summon35()
    {
        return TrySummon(35, CostFor35);
    }

    private List<CompanionInstance> TrySummon(int count, int cost)
    {
        var player = PlayerManager.Instance;
        if (player == null || !player.SpendCompanionShard(cost)) return null;

        List<CompanionInstance> results = new List<CompanionInstance>();
        for (int i = 0; i < count; i++)
        {
            var data = RollRandomCompanion();
            var instance = new CompanionInstance { Data = data };
            CompanionManager.Instance.AddCompanion(instance);
            results.Add(instance);
        }

        return results;
    }

    private CompanionData RollRandomCompanion()
    {
        float roll = Random.value;
        CompanionRarity rarity;

        if (roll <= MythicChance) rarity = CompanionRarity.Mythic;
        else if (roll <= MythicChance + LegendaryChance) rarity = CompanionRarity.Legendary;
        else if (roll <= MythicChance + LegendaryChance + EpicChance) rarity = CompanionRarity.Epic;
        else if (roll <= MythicChance + LegendaryChance + EpicChance + RareChance) rarity = CompanionRarity.Rare;
        else rarity = CompanionRarity.Common;

        List<CompanionData> filtered = CompanionPool.FindAll(c => c.Rarity == rarity);
        if (filtered.Count == 0) filtered = CompanionPool;

        return filtered[Random.Range(0, filtered.Count)];
    }
}
