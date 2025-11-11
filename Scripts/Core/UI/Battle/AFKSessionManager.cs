using UnityEngine;
using System;
using System.Collections;

public class AFKSessionManager : MonoBehaviour
{
    public static AFKSessionManager Instance;

    [Header("References")]
    public PlayerManager player;
    public EnemyManager enemy;
    public AFKSessionSummary summary;

    [Header("Settings")]
    public float tickInterval = 3f;
    public float maxSessionDuration = 3600f;
    [Range(0f, 1f)] public float shardDropChance = 0.05f;

    private bool isRunning = false;
    private float elapsed = 0f;
    private Coroutine sessionLoop;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartAFKSession()
    {
        if (isRunning)
        {
            Debug.Log("⚠️ AFK session already running.");
            return;
        }

        if (player == null || enemy == null || summary == null)
        {
            Debug.LogError("❌ Missing references in AFKSessionManager.");
            return;
        }

        Debug.Log("🌙 Starting AFK Battle Session...");
        isRunning = true;
        elapsed = 0f;

        summary.BeginSession();
        sessionLoop = StartCoroutine(SessionLoop());
    }

    public void StopAFKSession()
    {
        if (!isRunning) return;

        Debug.Log("🛑 Ending AFK Battle Session...");
        isRunning = false;

        if (sessionLoop != null)
            StopCoroutine(sessionLoop);

        summary.EndSession();
    }

    private IEnumerator SessionLoop()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;

            var eStats = enemy.GenerateCurrentStageEnemy();
            player.GrantMonsterXP(eStats.Level);
            player.AddCredits(eStats.CreditReward);

            if (UnityEngine.Random.value < shardDropChance)
            {
                player.AddQuantumShards(1);
                Debug.Log("💎 Quantum Shard dropped!");
            }

            enemy.TotalEnemiesDefeated++;

            Debug.Log($"⚔️ AFK Tick → +{eStats.XPReward} XP, +{eStats.CreditReward:N0} Cr, Total Kills: {enemy.TotalEnemiesDefeated}");

            if (elapsed >= maxSessionDuration)
            {
                StopAFKSession();
            }
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused && isRunning)
        {
            Debug.Log("⏸️ Game paused, ending AFK session safely...");
            StopAFKSession();
        }
    }

    void OnApplicationQuit()
    {
        if (isRunning)
        {
            Debug.Log("🛑 Game quitting, ending AFK session safely...");
            StopAFKSession();
        }
    }
}
