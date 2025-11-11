using System.Collections.Generic;
using UnityEngine;

public class SkillSummonManager : MonoBehaviour
{
    public static SkillSummonManager Instance;

    [Header("Skill Pool")]
    public List<SkillData> SkillPool = new List<SkillData>();

    [Header("Vending Costs")]
    public int TicketsFor15 = 15;
    public int TicketsFor35 = 30;

    [Header("Rarity Chances (sum ≈ 1.0)")]
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

    // Public API
    public List<SkillInstance> Summon15()
    {
        return TrySummonBundle(TicketsFor15, 15);
    }

    public List<SkillInstance> Summon35()
    {
        return TrySummonBundle(TicketsFor35, 35);
    }

    // Core Summon Logic
    private List<SkillInstance> TrySummonBundle(int ticketCost, int pulls)
    {
        var player = PlayerManager.Instance;
        var skillMgr = SkillManager.Instance;

        if (player == null || skillMgr == null)
        {
            Debug.LogWarning("⚠️ Missing PlayerManager or SkillManager.");
            return null;
        }

        if (!player.SpendSkillTickets(ticketCost))
        {
            Debug.Log($"❌ Not enough Skill Tickets. Need {ticketCost}.");
            return null;
        }

        List<SkillInstance> results = new List<SkillInstance>();

        for (int i = 0; i < pulls; i++)
        {
            SkillData rolled = RollSkillFromPool();
            if (rolled == null) continue;

            var owned = skillMgr.FindOwnedByName(rolled.SkillName);
            if (owned == null)
            {
                skillMgr.AddNewSkill(rolled);
                owned = skillMgr.FindOwnedByName(rolled.SkillName);
            }
            else
            {
                skillMgr.AddDupeSkill(rolled);
            }

            // XP reward for the player for using the machine
            int xp = Mathf.RoundToInt(30f * (1f + player.PlayerLevel * 0.03f));
            player.AddXP(xp, XPRewardType.Quest);

            results.Add(owned);
        }

        Debug.Log($"📦 Skill Vending used. Cost {ticketCost} → Got {results.Count} skill(s).");
        return results;
    }

    private SkillData RollSkillFromPool()
    {
        float roll = Random.value;
        SkillRarity rarity;

        if (roll <= MythicChance) rarity = SkillRarity.Mythic;
        else if (roll <= MythicChance + LegendaryChance) rarity = SkillRarity.Legendary;
        else if (roll <= MythicChance + LegendaryChance + EpicChance) rarity = SkillRarity.Epic;
        else if (roll <= MythicChance + LegendaryChance + EpicChance + RareChance) rarity = SkillRarity.Rare;
        else rarity = SkillRarity.Common;

        List<SkillData> filtered = SkillPool.FindAll(s => s.Rarity == rarity);
        if (filtered.Count == 0) filtered = SkillPool;

        return filtered.Count > 0 ? filtered[Random.Range(0, filtered.Count)] : null;
    }
}
