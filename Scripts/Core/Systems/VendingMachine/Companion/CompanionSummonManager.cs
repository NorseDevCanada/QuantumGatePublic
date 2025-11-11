using System.Collections.Generic;
using UnityEngine;

public class CompanionSummonManager : MonoBehaviour
{
    public static CompanionSummonManager Instance;

    [Header("Summon Data")]
    public CompanionSummonData summonData;

    [Header("XP Rewards")]
    [Tooltip("XP per summon (scales slightly with player level).")]
    public int BaseXPPerSummon = 50;
    public float XPLevelFactor = 0.05f;

    [Header("Vending Machine Settings")]
    [Tooltip("Tickets required per vending bundle.")]
    public int TicketsFor15Pulls = 15;
    public int TicketsFor35Pulls = 30;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------------------------------------------------------------
    // 🎰 Public Summon Entry Points
    // ------------------------------------------------------------------------

    public List<CompanionInstance> Summon15()
    {
        return SummonBundle(TicketsFor15Pulls, 15);
    }

    public List<CompanionInstance> Summon35()
    {
        return SummonBundle(TicketsFor35Pulls, 35);
    }

    // ------------------------------------------------------------------------
    // 🧩 Core Summon Logic
    // ------------------------------------------------------------------------

    private List<CompanionInstance> SummonBundle(int ticketCost, int pulls)
    {
        var player = PlayerManager.Instance;
        var compMgr = CompanionManager.Instance;

        if (player == null || compMgr == null || summonData == null)
        {
            Debug.LogWarning("⚠️ Missing manager or summon data.");
            return null;
        }

        // Check ticket balance
        if (!player.SpendCompanionTickets(ticketCost))
        {
            Debug.Log($"❌ Not enough Companion Tickets! Need {ticketCost}.");
            return null;
        }

        List<CompanionInstance> results = new();
        for (int i = 0; i < pulls; i++)
        {
            CompanionData data = summonData.GetRandomCompanion();
            if (data == null) continue;

            CompanionInstance newCompanion = compMgr.AddNewCompanion(data);

            // XP reward per summon
            int xp = Mathf.RoundToInt(BaseXPPerSummon * (1f + (player.PlayerLevel * XPLevelFactor)));
            player.AddXP(xp, XPRewardType.Quest);

            results.Add(newCompanion);
        }

        Debug.Log($"🎰 Vending Machine Activated! Cost: {ticketCost} | Pulls: {pulls} | Gained {results.Count} companions.");
        return results;
    }
}
