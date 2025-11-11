using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Core Wallets")]
    public float Credits;
    public int QuantumShards;
    public int CompanionShards;
    public int CompanionTickets;
    public int SkillTickets;
    public int PlayerGems;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------------------------------------------------------------
    // 💰 Core Adders
    // ------------------------------------------------------------------------
    public void AddCredits(float amount)
    {
        Credits += amount;
        Debug.Log($"💰 +{amount:N0} Credits → Total: {Credits:N0}");
    }

    public void AddQuantumShards(int amount)
    {
        QuantumShards += amount;
        Debug.Log($"🔷 +{amount} Quantum Shards → Total: {QuantumShards}");
    }

    public void AddCompanionShards(int amount)
    {
        CompanionShards += amount;
        Debug.Log($"🐾 +{amount} Companion Shards → Total: {CompanionShards}");
    }

    public void AddCompanionTickets(int amount)
    {
        CompanionTickets += amount;
        Debug.Log($"🎟️ +{amount} Companion Tickets → Total: {CompanionTickets}");
    }

    public void AddSkillTickets(int amount)
    {
        SkillTickets += amount;
        Debug.Log($"🎫 +{amount} Skill Tickets → Total: {SkillTickets}");
    }

    public void AddGems(int amount)
    {
        PlayerGems += amount;
        Debug.Log($"💎 +{amount} Gems → Total: {PlayerGems}");
    }

    // ------------------------------------------------------------------------
    // 💸 Spending Logic
    // ------------------------------------------------------------------------
    public bool SpendCredits(float amount)
    {
        if (Credits < amount) return false;
        Credits -= amount;
        Debug.Log($"🪙 Spent {amount:N0} Credits → Remaining: {Credits:N0}");
        return true;
    }

    public bool SpendQuantumShards(int amount)
    {
        if (QuantumShards < amount) return false;
        QuantumShards -= amount;
        Debug.Log($"🔮 Spent {amount} Quantum Shards → Remaining: {QuantumShards}");
        return true;
    }

    public bool SpendCompanionShards(int amount)
    {
        if (CompanionShards < amount) return false;
        CompanionShards -= amount;
        Debug.Log($"🐾 Spent {amount} Companion Shards → Remaining: {CompanionShards}");
        return true;
    }

    public bool SpendCompanionTickets(int amount)
    {
        if (CompanionTickets < amount) return false;
        CompanionTickets -= amount;
        Debug.Log($"🎰 Spent {amount} Companion Tickets → Remaining: {CompanionTickets}");
        return true;
    }

    public bool SpendSkillTickets(int amount)
    {
        if (SkillTickets < amount) return false;
        SkillTickets -= amount;
        Debug.Log($"🧪 Spent {amount} Skill Tickets → Remaining: {SkillTickets}");
        return true;
    }

    public bool SpendGems(int amount)
    {
        if (PlayerGems < amount) return false;
        PlayerGems -= amount;
        Debug.Log($"💎 Spent {amount} Gems → Remaining: {PlayerGems}");
        return true;
    }

    // ------------------------------------------------------------------------
    // 🧾 Getters
    // ------------------------------------------------------------------------
    public float GetCredits() => Credits;
    public int GetQuantumShards() => QuantumShards;
    public int GetCompanionShards() => CompanionShards;
    public int GetCompanionTickets() => CompanionTickets;
    public int GetSkillTickets() => SkillTickets;
    public int GetGems() => PlayerGems;
}
