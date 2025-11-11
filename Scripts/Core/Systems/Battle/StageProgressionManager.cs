using System.Collections;
using UnityEngine;

public class StageProgressionManager : MonoBehaviour
{
    public static StageProgressionManager Instance;

    [Header("Stage Settings")]
    public int CurrentStage = 1;
    public int EnemiesPerStage = 10;
    public float TimePerEnemy = 2.5f;
    public float DifficultyMultiplierPerStage = 1.08f;

    [Header("Boss Settings")]
    public int BossStageInterval = 10;
    public float BossRewardMultiplier = 2.0f;
    public float BossStatMultiplier = 3.0f;
    public float BossExtraDelay = 2.0f;

    [Header("Companion Integration")]
    public float CompanionDamageFactor = 0.35f;
    public float CompanionPassiveFactor = 0.25f;

    [Header("State")]
    public int EnemiesDefeated = 0;
    public bool IsRunning = false;
    public bool IsPaused = false;

    private Coroutine stageRoutine;
    private PlayerManager player;
    private CompanionManager companions;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = PlayerManager.Instance;
        companions = CompanionManager.Instance;

        if (player == null)
        {
            Debug.LogWarning("⚠️ No PlayerManager found for StageProgressionManager.");
            return;
        }

        StartStageLoop();
    }

    public void StartStageLoop()
    {
        if (IsRunning || IsPaused) return;
        stageRoutine = StartCoroutine(RunStageLoop());
    }

    public void StopStageLoop()
    {
        if (stageRoutine != null)
        {
            StopCoroutine(stageRoutine);
            stageRoutine = null;
        }
        IsRunning = false;
    }

    private IEnumerator RunStageLoop()
    {
        IsRunning = true;

        while (true)
        {
            if (IsPaused)
            {
                yield return null;
                continue;
            }

            bool isBossStage = IsBossStage(CurrentStage);
            int enemiesToDefeat = isBossStage ? 1 : EnemiesPerStage;

            if (EnemiesDefeated >= enemiesToDefeat)
            {
                AdvanceStage();
                yield return new WaitForSeconds(2f);
                continue;
            }

            EnemyType enemyType = isBossStage ? EnemyType.Boss : EnemyType.Normal;
            EnemyManager enemyMgr = EnemyManager.Instance;
            
            EnemyStats enemy = enemyMgr != null 
                ? enemyMgr.GenerateCurrentStageEnemy()
                : new EnemyStats();

            if (isBossStage)
            {
                enemy.Health *= BossStatMultiplier;
                enemy.Attack *= BossStatMultiplier * 0.8f;
                enemy.Defense *= BossStatMultiplier * 0.6f;
            }

            yield return new WaitForSeconds(TimePerEnemy);

            float totalDamage = player.CombatPower;
            if (companions != null && companions.EquippedCompanions.Count > 0)
            {
                foreach (var comp in companions.EquippedCompanions)
                {
                    totalDamage += comp.GetCombatPower() * CompanionDamageFactor;
                }
            }

            EnemiesDefeated++;

            int baseXP = Mathf.RoundToInt(enemy.XPReward * (isBossStage ? BossRewardMultiplier : 1f));
            float baseCredits = enemy.CreditReward * (isBossStage ? BossRewardMultiplier : 1f);

            float xpMultiplier = 1f;
            float creditMultiplier = 1f;

            if (SkillManager.Instance != null)
            {
                xpMultiplier += SkillManager.Instance.GetXPBonusPercent() / 100f;
                creditMultiplier += SkillManager.Instance.GetCreditBonusPercent() / 100f;
            }

            if (companions != null)
            {
                foreach (var comp in companions.EquippedCompanions)
                {
                    xpMultiplier += comp.Data.PlayerCPBonusPercent * CompanionPassiveFactor;
                    creditMultiplier += comp.Data.PlayerCPBonusPercent * CompanionPassiveFactor;
                }
            }

            int finalXP = Mathf.RoundToInt(baseXP * xpMultiplier);
            float finalCredits = baseCredits * creditMultiplier;

            player.AddXP(finalXP, XPRewardType.MonsterKill);
            player.AddCredits(finalCredits);

            float shardChance = isBossStage ? 0.10f : 0.03f;
            if (Random.value <= shardChance)
            {
                player.AddQuantumShards(1);
                Debug.Log("💠 Quantum Shard +1!");
            }

            if (companions != null)
            {
                foreach (var comp in companions.EquippedCompanions)
                {
                    comp.AddXP(Mathf.RoundToInt(finalXP * 0.15f));
                }
            }

            if (isBossStage)
                Debug.Log($"👑 Boss defeated! Stage {CurrentStage} cleared. +{finalXP} XP, +{finalCredits:N0} Cr");
            else
                Debug.Log($"[Stage {CurrentStage}] Enemy {EnemiesDefeated}/{enemiesToDefeat} defeated. +{finalXP} XP, +{finalCredits:N0} Cr");

            yield return new WaitForSeconds(isBossStage ? BossExtraDelay : 0f);
        }
    }

    private void AdvanceStage()
    {
        CurrentStage++;
        EnemiesDefeated = 0;
        TimePerEnemy = Mathf.Max(1.25f, TimePerEnemy * 0.985f);
        Debug.Log($"🚀 Advanced to Stage {CurrentStage}{(IsBossStage(CurrentStage) ? " (BOSS)" : "")}!");
    }

    public bool IsBossStage(int stage)
    {
        return stage % BossStageInterval == 0;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            IsPaused = true;
            Debug.Log("⏸️ Stage progression paused (lost focus).");
        }
        else
        {
            IsPaused = false;
            Debug.Log("▶️ Stage progression resumed.");
        }
    }
}
