using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Stage Progression")]
    public int CurrentStage = 1;
    public int MaxStage = 1000;
    public int EnemiesPerStage = 10;
    public int EnemiesDefeatedInStage = 0;
    public int TotalEnemiesDefeated = 0;

    [Header("Difficulty Curves")]
    [Tooltip("Enemy power multiplier per stage.")]
    public AnimationCurve StagePowerCurve = AnimationCurve.EaseInOut(1, 1, 1000, 25f);

    [Tooltip("Boss every N stages.")]
    public int BossEvery = 10;

    private PlayerManager player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = PlayerManager.Instance;
    }

    // ------------------------------------------------------------------------
    // 🧮 Enemy Generation
    // ------------------------------------------------------------------------
    public EnemyStats GenerateCurrentStageEnemy()
    {
        EnemyType type = GetEnemyTypeForStage(CurrentStage);
        EnemyStats stats = GenerateEnemyStatsForStage(CurrentStage, type);
        return stats;
    }

    private EnemyType GetEnemyTypeForStage(int stage)
    {
        if (stage % BossEvery == 0) return EnemyType.Boss;
        if (stage % 5 == 0) return EnemyType.Elite;
        return EnemyType.Normal;
    }

    private EnemyStats GenerateEnemyStatsForStage(int stage, EnemyType type)
    {
        int baseLevel = Mathf.RoundToInt(stage * 1.2f);
        float difficultyMult = StagePowerCurve.Evaluate(stage);
        float health = (100f + (stage * 20f)) * difficultyMult;
        float attack = (15f + (stage * 5f)) * difficultyMult;
        float defense = (5f + (stage * 2f)) * difficultyMult;

        // Rewards (based on stage)
        int xp = Mathf.RoundToInt(stage * 50f * difficultyMult);
        float credits = Mathf.Round(stage * 100f * difficultyMult);

        // Apply type modifiers
        switch (type)
        {
            case EnemyType.Elite:
                health *= 2.5f;
                attack *= 2f;
                defense *= 1.5f;
                xp = Mathf.RoundToInt(xp * 2f);
                credits *= 2f;
                break;

            case EnemyType.Boss:
                health *= 6f;
                attack *= 3.5f;
                defense *= 2.5f;
                xp = Mathf.RoundToInt(xp * 5f);
                credits *= 5f;
                break;
        }

        EnemyStats e = new EnemyStats
        {
            Level = baseLevel,
            Health = health,
            Attack = attack,
            Defense = defense,
            XPReward = xp,
            CreditReward = credits
        };

        // Log preview
        Debug.Log($"[Enemy] Stage {stage} {type} → L{e.Level} | HP:{e.Health:N0} | ATK:{e.Attack:N0} | DEF:{e.Defense:N0}");

        return e;
    }

    // ------------------------------------------------------------------------
    // ⚔️ Stage Progression
    // ------------------------------------------------------------------------
    public void OnEnemyDefeated()
    {
        EnemiesDefeatedInStage++;
        TotalEnemiesDefeated++;

        if (EnemiesDefeatedInStage >= EnemiesPerStage)
        {
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        EnemiesDefeatedInStage = 0;
        if (CurrentStage < MaxStage)
        {
            CurrentStage++;
            Debug.Log($"🚀 Advanced to Stage {CurrentStage}");
        }
        else
        {
            Debug.Log("🏆 Max Stage reached!");
        }
    }

    // ------------------------------------------------------------------------
    // 🧩 Reset (for testing / debugging)
    // ------------------------------------------------------------------------
    public void ResetStages()
    {
        CurrentStage = 1;
        EnemiesDefeatedInStage = 0;
        TotalEnemiesDefeated = 0;
    }
}
