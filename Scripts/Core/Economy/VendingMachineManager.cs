// Scripts/Economy/VendingMachineManager.cs
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineManager : MonoBehaviour
{
    public static VendingMachineManager Instance;

    [Header("Pools")]
    public List<CompanionData> CompanionPool = new List<CompanionData>();
    public List<SkillData> SkillPool = new List<SkillData>();

    [Header("Costs")]
    public int CompanionCost15 = 15;
    public int CompanionCost35 = 30;
    public int SkillCost15 = 15;
    public int SkillCost35 = 30;

    [Header("Rarity Odds (percentages)")]
    [Tooltip("Drop chances for Common → Mythic. Must total 100.")]
    public float[] CompanionRarityChances = { 60f, 25f, 10f, 4f, 1f };
    public float[] SkillRarityChances = { 60f, 25f, 10f, 4f, 1f };

    private PlayerManager player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = PlayerManager.Instance;
    }

    // ------------------------------------------------------------------------
    // 🧩 COMPANIONS
    // ------------------------------------------------------------------------
    public List<CompanionInstance> SummonCompanions15() => SummonCompanions(CompanionCost15, 15);
    public List<CompanionInstance> SummonCompanions35() => SummonCompanions(CompanionCost35, 35);

    private List<CompanionInstance> SummonCompanions(int ticketCost, int pulls)
    {
        player ??= PlayerManager.Instance;
        if (player == null) return new List<CompanionInstance>();

        if (!player.SpendCompanionTickets(ticketCost))
        {
            Debug.Log($"❌ Not enough companion tickets. Need {ticketCost}.");
            return new List<CompanionInstance>();
        }

        if (CompanionManager.Instance == null)
        {
            Debug.LogError("❌ No CompanionManager instance found.");
            return new List<CompanionInstance>();
        }

        List<CompanionInstance> results = new List<CompanionInstance>();

        for (int i = 0; i < pulls; i++)
        {
            CompanionData rolled = RollCompanion();
            if (rolled == null) continue;

            var owned = CompanionManager.Instance.FindOwnedByID(rolled.GetInstanceID());
            if (owned == null)
            {
                CompanionManager.Instance.AddNewCompanion(rolled);
                owned = CompanionManager.Instance.FindOwnedByID(rolled.GetInstanceID());
                Debug.Log($"🆕 New Companion → {rolled.CompanionName}");
            }
            else
            {
                CompanionManager.Instance.AddDupeCompanion(rolled);
                Debug.Log($"✨ Duplicate Companion → +XP to {rolled.CompanionName}");
            }

            results.Add(owned);
        }

        Debug.Log($"🎰 Companion Summon Complete → {results.Count}/{pulls} pulled.");
        return results;
    }

    private CompanionData RollCompanion()
    {
        if (CompanionPool.Count == 0) return null;

        CompanionRarity rarity = GetWeightedCompanionRarity();
        List<CompanionData> filtered = CompanionPool.FindAll(c => c.Rarity == rarity);

        if (filtered.Count == 0)
            filtered = CompanionPool; // fallback to all

        return filtered[Random.Range(0, filtered.Count)];
    }

    private CompanionRarity GetWeightedCompanionRarity()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < CompanionRarityChances.Length; i++)
        {
            cumulative += CompanionRarityChances[i];
            if (roll <= cumulative)
                return (CompanionRarity)i;
        }

        return CompanionRarity.Common;
    }

    // ------------------------------------------------------------------------
    // 🧠 SKILLS
    // ------------------------------------------------------------------------
    public List<SkillInstance> SummonSkills15() => SummonSkills(SkillCost15, 15);
    public List<SkillInstance> SummonSkills35() => SummonSkills(SkillCost35, 35);

    private List<SkillInstance> SummonSkills(int ticketCost, int pulls)
    {
        player ??= PlayerManager.Instance;
        if (player == null) return new List<SkillInstance>();

        if (!player.SpendCompanionTickets(ticketCost)) // unified ticket pool
        {
            Debug.Log($"❌ Not enough skill tickets. Need {ticketCost}.");
            return new List<SkillInstance>();
        }

        if (SkillManager.Instance == null)
        {
            Debug.LogError("❌ No SkillManager instance found.");
            return new List<SkillInstance>();
        }

        List<SkillInstance> results = new List<SkillInstance>();

        for (int i = 0; i < pulls; i++)
        {
            SkillData rolled = RollSkill();
            if (rolled == null) continue;

            var owned = SkillManager.Instance.FindOwnedByName(rolled.SkillName);
            if (owned == null)
            {
                SkillManager.Instance.AddNewSkill(rolled);
                owned = SkillManager.Instance.FindOwnedByName(rolled.SkillName);
                Debug.Log($"🆕 New Skill → {rolled.SkillName}");
            }
            else
            {
                SkillManager.Instance.AddDupeSkill(rolled);
                Debug.Log($"✨ Duplicate Skill → +XP to {rolled.SkillName}");
            }

            results.Add(owned);
        }

        Debug.Log($"🎰 Skill Summon Complete → {results.Count}/{pulls} pulled.");
        return results;
    }

    private SkillData RollSkill()
    {
        if (SkillPool.Count == 0) return null;

        SkillRarity rarity = GetWeightedSkillRarity();
        List<SkillData> filtered = SkillPool.FindAll(s => s.Rarity == rarity);

        if (filtered.Count == 0)
            filtered = SkillPool; // fallback if pool incomplete

        return filtered[Random.Range(0, filtered.Count)];
    }

    private SkillRarity GetWeightedSkillRarity()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < SkillRarityChances.Length; i++)
        {
            cumulative += SkillRarityChances[i];
            if (roll <= cumulative)
                return (SkillRarity)i;
        }

        return SkillRarity.Common;
    }
}
