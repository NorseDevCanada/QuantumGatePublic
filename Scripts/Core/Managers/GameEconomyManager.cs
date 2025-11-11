using UnityEngine;

public class GameEconomyManager : MonoBehaviour
{
    public static GameEconomyManager Instance;

    [Header("Reward Multipliers")]
    public float GlobalXPMultiplier = 1f;
    public float GlobalCreditMultiplier = 1f;
    public float GlobalDropRateMultiplier = 1f;

    [Header("Drop Chances (per enemy)")]
    public float QuantumShardChance = 0.03f;
    public float CompanionShardChance = 0.02f;
    public float SkillTicketChance = 0.015f;
    public float CompanionTicketChance = 0.012f;

    [Header("Boss Multipliers")]
    public float BossRewardMultiplier = 2.0f;
    public float BossDropBonus = 2.5f;

    private PlayerManager player;
    private CurrencyManager wallet;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = PlayerManager.Instance;
        wallet = CurrencyManager.Instance;
    }

    // ------------------------------------------------------------------------
    // 🎁 Rewards
    // ------------------------------------------------------------------------
    public void GrantEnemyRewards(EnemyStats enemy, bool isBoss)
    {
        if (player == null) player = PlayerManager.Instance;
        if (wallet == null) wallet = CurrencyManager.Instance;
        if (player == null || wallet == null) return;

        float rewardMult = isBoss ? BossRewardMultiplier : 1f;

        int baseXP = Mathf.RoundToInt(enemy.XPReward * rewardMult * GlobalXPMultiplier);
        float baseCredits = enemy.CreditReward * rewardMult * GlobalCreditMultiplier;

        float skillXPBonus = SkillManager.Instance?.GetXPBonusPercent() ?? 0f;
        float skillCreditBonus = SkillManager.Instance?.GetCreditBonusPercent() ?? 0f;

        int finalXP = Mathf.RoundToInt(baseXP * (1f + skillXPBonus / 100f));
        float finalCredits = baseCredits * (1f + skillCreditBonus / 100f);

        player.AddXP(finalXP, XPRewardType.MonsterKill);
        wallet.AddCredits(finalCredits);

        TryDropRewards(isBoss);
        Debug.Log($"🎖️ Rewards: +{finalXP} XP | +{finalCredits:N0} Cr");
    }

    private void TryDropRewards(bool isBoss)
    {
        if (wallet == null) return;

        float bossMult = isBoss ? BossDropBonus : 1f;

        if (Random.value <= QuantumShardChance * GlobalDropRateMultiplier * bossMult)
            wallet.AddQuantumShards(1);

        if (Random.value <= CompanionShardChance * GlobalDropRateMultiplier * bossMult)
            wallet.AddCompanionShards(1);

        if (Random.value <= SkillTicketChance * GlobalDropRateMultiplier * bossMult)
            wallet.AddSkillTickets(1);

        if (Random.value <= CompanionTicketChance * GlobalDropRateMultiplier * bossMult)
            wallet.AddCompanionTickets(1);
    }

    public void GrantEventRewards(int xp, float credits, int quantum = 0, int skillTickets = 0)
    {
        if (player == null || wallet == null) return;

        player.AddXP(Mathf.RoundToInt(xp * GlobalXPMultiplier), XPRewardType.Quest);
        wallet.AddCredits(credits * GlobalCreditMultiplier);
        if (quantum > 0) wallet.AddQuantumShards(quantum);
        if (skillTickets > 0) wallet.AddSkillTickets(skillTickets);
    }
}
