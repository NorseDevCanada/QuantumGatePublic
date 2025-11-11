using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Combat Settings")]
    public int WavesPerLevel = 5;
    public int CurrentWave = 1;
    public int InfinityLevel = 1;

    [Header("Gear Drop")]
    public GearDropSystem gearDropSystem; // Assign in inspector

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AutoResolveWave()
    {
        // 1. Calculate total team power
        int playerPower = PlayerManager.Instance.GetTotalPower();
        int companionPower = CompanionManager.Instance.GetTotalCompanionPower();
        int totalTeamPower = playerPower + companionPower;

        // 2. Reward credits
        int rewardCredits = totalTeamPower * CurrentWave;
        CurrencyManager.Instance.AddCredits(rewardCredits);
        Debug.Log($"Wave {CurrentWave} (Infinity {InfinityLevel}) cleared! Credits earned: {rewardCredits}");

        // 3. Gear drop
        if (gearDropSystem != null)
        {
            Gear droppedGear = gearDropSystem.GetDroppedGear();
            if (droppedGear != null)
            {
                // Auto-equip to player (Weapon slot for testing)
                PlayerManager.Instance.Weapon = droppedGear;
                Debug.Log($"Auto-equipped {droppedGear.GearName} (Power {droppedGear.PowerBonus})");
            }
        }

        // 4. Wave progression
        CurrentWave++;
        if (CurrentWave > WavesPerLevel)
        {
            Debug.Log($"Boss defeated! Moving to next Infinity sector.");
            CurrentWave = 1;
            InfinityLevel++;
        }
    }
}
