using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Gear System")]
    public GearSlot[] GearSlots = new GearSlot[10];

    [Header("Progression")]
    public int PlayerLevel = 1;
    public int CurrentXP = 0;
    public int XPToNextLevel = 100;
    public PlayerStats Stats = new PlayerStats();

    [Header("Economy")]
    public float TotalCredits = 0;

    [Header("Special Currencies")]
    [Tooltip("Used for activating the Quantum Gate (Lamp) system.")]
    public int QuantumShards = 0;

    [Tooltip("Used for summoning companions or future gacha systems.")]
    public int CompanionShards = 0;

    [Tooltip("Used for the Companion Vending Machine (ticket system).")]
    public int CompanionTickets = 0;

    [Tooltip("Used for the Skill Vending Machine.")]
    public int SkillTickets = 0;

    [Tooltip("Premium currency placeholder if needed later.")]
    public int PlayerGems = 0;

    [Header("Combat Power")]
    public int CombatPower = 0;
    public PlayerClassType ActiveClass = PlayerClassType.PistolSpecialist;

    [Header("Scaling Curves")]
    [Tooltip("Multiplier applied to base stats per level.")]
    public AnimationCurve PowerCurve = AnimationCurve.EaseInOut(1, 1, 100, 3);
    public AnimationCurve HealthCurve = AnimationCurve.EaseInOut(1, 1, 100, 4);
    public AnimationCurve SpeedCurve = AnimationCurve.EaseInOut(1, 1, 100, 2);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        string[] defaultNames =
        {
            "Pistol", "Rifle", "Helmet", "Armor", "Boots",
            "Gloves", "Shield", "Accessory1", "Accessory2", "Accessory3"
        };

        for (int i = 0; i < GearSlots.Length; i++)
            GearSlots[i] = new GearSlot { SlotName = defaultNames[i], EquippedGear = null };

        XPToNextLevel = XPProgression.GetXPForLevel(PlayerLevel);
        RecalculateStats();
    }

    // ------------------------------------------------------------------------
    // 🧩 XP & Progression System
    // ------------------------------------------------------------------------
    public void AddXP(int amount, XPRewardType source = XPRewardType.Quest)
    {
        if (PlayerLevel >= XPProgression.MAX_LEVEL)
        {
            Debug.Log("⚠️ Max level reached.");
            return;
        }

        CurrentXP += amount;
        Debug.Log($"[XP] +{amount} XP from {source} → {CurrentXP}/{XPToNextLevel}");

        while (CurrentXP >= XPToNextLevel && PlayerLevel < XPProgression.MAX_LEVEL)
        {
            CurrentXP -= XPToNextLevel;
            PlayerLevel++;
            XPToNextLevel = XPProgression.GetXPForLevel(PlayerLevel);
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        Debug.Log($"🆙 Player reached Level {PlayerLevel}!");
        RecalculateStats();

        if (PlayerClassManager.Instance != null)
            PlayerClassManager.Instance.PlayerLevel = PlayerLevel;
    }

    // ------------------------------------------------------------------------
    // 💰 Credit & Currency Helpers
    // ------------------------------------------------------------------------
    public void AddCredits(float amount)
    {
        TotalCredits += amount;
        Debug.Log($"💰 +{amount:N0} credits (Total: {TotalCredits:N0})");
    }

    public void AddQuantumShards(int amount)
    {
        QuantumShards += amount;
        Debug.Log($"🔷 +{amount} Quantum Shards → Total: {QuantumShards}");
    }

    public bool SpendQuantumShard()
    {
        if (QuantumShards <= 0)
            return false;

        QuantumShards--;
        Debug.Log($"🔮 Quantum Gate activated. Remaining shards: {QuantumShards}");
        return true;
    }

    public void AddCompanionShards(int amount)
    {
        CompanionShards += amount;
        Debug.Log($"🧩 +{amount} Companion Shards → Total: {CompanionShards}");
    }

    public bool SpendCompanionShard(int cost = 1)
    {
        if (CompanionShards < cost)
            return false;

        CompanionShards -= cost;
        Debug.Log($"🐾 Companion summon used {cost} shard(s). Remaining: {CompanionShards}");
        return true;
    }

    public void AddCompanionTickets(int amount)
    {
        CompanionTickets += amount;
        Debug.Log($"🎟️ +{amount} Companion Tickets → Total: {CompanionTickets}");
    }

    public bool SpendCompanionTickets(int amount)
    {
        if (CompanionTickets < amount)
            return false;

        CompanionTickets -= amount;
        Debug.Log($"🎰 Spent {amount} Companion Tickets → Remaining: {CompanionTickets}");
        return true;
    }

    public void AddSkillTickets(int amount)
    {
        SkillTickets += amount;
        Debug.Log($"🎫 +{amount} Skill Tickets → Total: {SkillTickets}");
    }

    public bool SpendSkillTickets(int amount)
    {
        if (SkillTickets < amount)
            return false;

        SkillTickets -= amount;
        Debug.Log($"🧪 Spent {amount} Skill Tickets → Remaining: {SkillTickets}");
        return true;
    }

    // ------------------------------------------------------------------------
    // 🏆 Reward Examples
    // ------------------------------------------------------------------------
    public void GrantMonsterXP(int monsterLevel)
    {
        int xpGain = Mathf.RoundToInt(monsterLevel * 15f);
        AddXP(xpGain, XPRewardType.MonsterKill);
    }

    public void GrantGearSaleXP(Gear soldGear)
    {
        int xpGain = Mathf.RoundToInt(soldGear.GearLevel * 25f);
        AddXP(xpGain, XPRewardType.GearSold);
        AddCredits(soldGear.SellValue);
    }

    public void GrantIdleOnlineRewards(float minutesOnline)
    {
        int xpGain = Mathf.RoundToInt(minutesOnline * 40f);
        float credits = minutesOnline * 200f;
        AddXP(xpGain, XPRewardType.IdleOnline);
        AddCredits(credits);
    }

    public void GrantOfflineRewards(float hoursOffline)
    {
        int xpGain = Mathf.RoundToInt(hoursOffline * 2000f);
        float credits = hoursOffline * 5000f;
        AddXP(xpGain, XPRewardType.IdleOffline);
        AddCredits(credits);
    }

    // ------------------------------------------------------------------------
    // ⚔️ Stat Calculation + Combat Power Integration
    // ------------------------------------------------------------------------
    public void RecalculateStats()
    {
        int gearPower = 0, gearHealth = 0, gearSpeed = 0;
        foreach (var slot in GearSlots)
        {
            if (slot.EquippedGear != null)
            {
                gearPower += slot.EquippedGear.PowerBonus;
                gearHealth += slot.EquippedGear.HealthBonus;
                gearSpeed += slot.EquippedGear.SpeedBonus;
            }
        }

        float powerScale = PowerCurve.Evaluate(PlayerLevel);
        float healthScale = HealthCurve.Evaluate(PlayerLevel);
        float speedScale = SpeedCurve.Evaluate(PlayerLevel);

        Stats.TotalPower = Mathf.RoundToInt((Stats.BasePower + gearPower) * powerScale);
        Stats.TotalHealth = Mathf.RoundToInt((Stats.BaseHealth + gearHealth) * healthScale);
        Stats.TotalSpeed = Mathf.RoundToInt((Stats.BaseSpeed + gearSpeed) * speedScale);

        // 🧠 Apply equipped skills
        if (SkillManager.Instance != null)
            SkillManager.Instance.ApplySkillBonusesToPlayer(this);

        Debug.Log($"[STATS] L{PlayerLevel} → Power:{Stats.TotalPower}  Health:{Stats.TotalHealth}  Speed:{Stats.TotalSpeed}");

        // 💥 Combat Power Calculation
        CombatPower = CombatPowerCalculator.CalculateCombatPower(Stats, ActiveClass);

        // 🧬 Add equipped companions’ combat contribution
        if (CompanionManager.Instance != null)
            CombatPower += CompanionManager.Instance.GetTotalEquippedCP();

        Debug.Log($"[CP] Calculated Combat Power: {CombatPower:N0} ({ActiveClass})");
    }

    // ------------------------------------------------------------------------
    // ⚙️ Gear Handling
    // ------------------------------------------------------------------------
    public void EquipGear(int slotIndex, Gear newGear)
    {
        if (slotIndex < 0 || slotIndex >= GearSlots.Length) return;
        GearSlots[slotIndex].EquippedGear = newGear;
        RecalculateStats();
    }

    public Gear GetGear(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= GearSlots.Length) return null;
        return GearSlots[slotIndex].EquippedGear;
    }

    public int GetTotalPower() => Stats.TotalPower;

    public int GetCombatPower() => CombatPower;
}
