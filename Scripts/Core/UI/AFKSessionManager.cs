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
    [Tooltip("How often (in seconds) enemies spawn and rewards tick.")]
    public float tickInterval = 3f;

    [Tooltip("Max duration of one AFK session (in seconds).")]
    public float maxSessionDuration = 3600f; // 1 hour for test, later could be 8h+

    [Tooltip("Chance to drop a Quantum Shard per kill.")]
    [Range(0f, 1f)] public float shardDropChance = 0.05f;

    private bool isRunning = false;
    private float elapsed = 0f;
    private Coroutine sessionLoop;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------------------------------------------------------------
    // ▶️ Start Session
    // ------------------------------------------------------------------------
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

        // 🔗 [SERVER HOOK: Start Session]
        /*
        SendSessionStartToServer(new AFKSessionStartData
        {
            playerId = player.PlayerId,
            startTimestamp = DateTime.UtcNow,
            stage = enemy.CurrentStage
        });
        */
    }

    // ------------------------------------------------------------------------
    // ⏹️ End Session
    // ------------------------------------------------------------------------
    public void StopAFKSession()
    {
        if (!isRunning) return;

        Debug.Log("🛑 Ending AFK Battle Session...");
        isRunning = false;

        if (sessionLoop != null)
            StopCoroutine(sessionLoop);

        summary.EndSession();

        // 🔗 [SERVER HOOK: End Session]
        /*
        SendSessionEndToServer(new AFKSessionEndData
        {
            playerId = player.PlayerId,
            durationSeconds = elapsed,
            totalKills = enemy.TotalEnemiesDefeated,
            totalXP = player.CurrentXP,
            totalCredits = player.TotalCredits,
            endTimestamp = DateTime.UtcNow
        });
        */
    }

    // ------------------------------------------------------------------------
    // 🔁 AFK Battle Loop
    // ------------------------------------------------------------------------
    private IEnumerator SessionLoop()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;

            // Spawn & "defeat" an enemy virtually
            var eStats = enemy.GenerateCurrentStageEnemy();
            player.GrantMonsterXP(eStats.Level);
            player.AddCredits(eStats.CreditReward);

            // Random Quantum Shard drop
            if (UnityEngine.Random.value < shardDropChance)
            {
                player.AddQuantumShard(1);
                Debug.Log("💎 Quantum Shard dropped!");
            }

            enemy.TotalEnemiesDefeated++;

            Debug.Log($"⚔️ AFK Tick → +{eStats.XPReward} XP, +{eStats.CreditReward:N0} Cr, Total Kills: {enemy.TotalEnemiesDefeated}");

            // Stop after max duration (safety)
            if (elapsed >= maxSessionDuration)
            {
                StopAFKSession();
            }
        }
    }

    // ------------------------------------------------------------------------
    // 📴 Pause / Quit Safety
    // ------------------------------------------------------------------------
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

    // ------------------------------------------------------------------------
    // 💬 [STUB FUNCTIONS] – future server sync
    // ------------------------------------------------------------------------
    /*
    private void SendSessionStartToServer(AFKSessionStartData data)
    {
        // Example:
        // NetworkManager.Instance.Send("AFK_SESSION_START", JsonUtility.ToJson(data));
        // or StartCoroutine(ServerAPI.Post("/afk/start", data));
    }

    private void SendSessionEndToServer(AFKSessionEndData data)
    {
        // Example:
        // NetworkManager.Instance.Send("AFK_SESSION_END", JsonUtility.ToJson(data));
        // or StartCoroutine(ServerAPI.Post("/afk/end", data));
    }
    */
}
