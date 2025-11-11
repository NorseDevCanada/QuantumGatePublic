using UnityEngine;
using System.Collections;

public class AFKBattleManager : MonoBehaviour
{
    public static AFKBattleManager Instance;

    [Header("AFK Settings")]
    [Tooltip("Seconds between each simulated enemy defeat.")]
    public float TimePerKill = 3f;

    [Tooltip("Multiplier for AFK reward rate vs active play.")]
    public float OfflineMultiplier = 0.5f;

    [Tooltip("Maximum hours of offline rewards that can be accumulated.")]
    public float MaxOfflineHours = 12f;

    private EnemyManager enemyManager;
    private PlayerManager playerManager;

    private float timer;
    private bool isRunning = false;

    // For offline time tracking
    private const string LastLogoutKey = "AFK_LastLogoutTime";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        enemyManager = EnemyManager.Instance;
        playerManager = PlayerManager.Instance;

        StartCoroutine(AFKLoop());
        GrantOfflineRewards();
    }

    // ------------------------------------------------------------------------
    // 🕹️ Active Battle Loop
    // ------------------------------------------------------------------------
    private IEnumerator AFKLoop()
    {
        isRunning = true;
        timer = 0f;

        while (isRunning)
        {
            timer += Time.deltaTime;

            if (timer >= TimePerKill)
            {
                timer = 0f;
                SimulateKill();
            }

            yield return null;
        }
    }

    private void SimulateKill()
    {
        if (enemyManager == null || playerManager == null) return;

        enemyManager.DefeatCurrentEnemy();
    }

    // ------------------------------------------------------------------------
    // 🌙 Offline Rewards
    // ------------------------------------------------------------------------
    private void GrantOfflineRewards()
    {
        if (!PlayerPrefs.HasKey(LastLogoutKey))
            return;

        double lastLogoutUnix = double.Parse(PlayerPrefs.GetString(LastLogoutKey));
        double currentUnix = GetUnixTimestamp();
        double secondsOffline = currentUnix - lastLogoutUnix;

        if (secondsOffline <= 0) return;

        float hoursOffline = Mathf.Min((float)(secondsOffline / 3600f), MaxOfflineHours);
        float effectiveMinutes = hoursOffline * 60f;
        float xpRate = 40f * OfflineMultiplier;        // Mirror PlayerManager.IdleOnline rate
        float creditRate = 200f * OfflineMultiplier;   // Mirror PlayerManager.IdleOnline rate

        int xpGain = Mathf.RoundToInt(effectiveMinutes * xpRate);
        float creditGain = effectiveMinutes * creditRate;

        playerManager.AddXP(xpGain, XPRewardType.IdleOffline);
        playerManager.AddCredits(creditGain);

        Debug.Log($"🌙 Offline Rewards: +{xpGain} XP, +{creditGain:N0} Cr over {hoursOffline:F1}h AFK time");
    }

    private void OnApplicationQuit()
    {
        SaveLogoutTime();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveLogoutTime();
    }

    private void SaveLogoutTime()
    {
        double timestamp = GetUnixTimestamp();
        PlayerPrefs.SetString(LastLogoutKey, timestamp.ToString());
        PlayerPrefs.Save();
    }

    private double GetUnixTimestamp()
    {
        return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
    }
}
