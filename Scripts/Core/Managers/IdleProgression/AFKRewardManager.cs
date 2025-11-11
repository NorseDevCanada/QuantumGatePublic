using UnityEngine;
using System;

public class AFKRewardManager : MonoBehaviour
{
    public static AFKRewardManager Instance;

    [Header("Reward Settings (per minute online)")]
    public int XPPerMinute = 40;
    public float CreditsPerMinute = 200f;

    [Header("Offline Reward Settings (per hour offline)")]
    public int XPPerHourOffline = 2000;
    public float CreditsPerHourOffline = 5000f;

    [Header("Limits & Modifiers")]
    [Tooltip("Maximum hours of offline time that can generate rewards.")]
    public float MaxOfflineHours = 8f;
    [Tooltip("How much the reward efficiency drops after the cap (0 = none, 1 = full loss).")]
    [Range(0f, 1f)] public float DiminishingReturnRate = 0.5f;

    [Header("Runtime State")]
    public float onlineTimer = 0f;
    private DateTime lastLogoutTime;
    private const string LogoutKey = "AFK_LastLogout";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GrantOfflineRewards();
    }

    void Update()
    {
        TrackOnlineIdle();
    }

    // ------------------------------------------------------------------------
    // 🕒 Online Idle Reward Logic
    // ------------------------------------------------------------------------
    void TrackOnlineIdle()
    {
        onlineTimer += Time.deltaTime;

        if (onlineTimer >= 60f)
        {
            onlineTimer -= 60f;
            PlayerManager.Instance.GrantIdleOnlineRewards(1f);
        }
    }

    // ------------------------------------------------------------------------
    // 💤 Offline Reward Logic
    // ------------------------------------------------------------------------
    void GrantOfflineRewards()
    {
        if (!PlayerPrefs.HasKey(LogoutKey))
        {
            Debug.Log("💤 No previous logout recorded — starting fresh session.");
            return;
        }

        string storedTime = PlayerPrefs.GetString(LogoutKey, "");
        if (DateTime.TryParse(storedTime, out lastLogoutTime))
        {
            TimeSpan offlineDuration = DateTime.UtcNow - lastLogoutTime;
            double hoursOffline = Math.Max(0, offlineDuration.TotalHours);

            if (hoursOffline > 0.05f)
            {
                float cappedHours = Mathf.Min((float)hoursOffline, MaxOfflineHours);
                float excessHours = Mathf.Max(0f, (float)hoursOffline - MaxOfflineHours);

                float effectiveHours = cappedHours + (excessHours * (1f - DiminishingReturnRate));

                Debug.Log($"⏰ Player was offline for {hoursOffline:F2}h (Effective: {effectiveHours:F2}h).");
                PlayerManager.Instance.GrantOfflineRewards(effectiveHours);

                if (hoursOffline > MaxOfflineHours)
                    Debug.Log($"⚠️ Max offline reward reached ({MaxOfflineHours}h). Diminished beyond that by {DiminishingReturnRate * 100f}%.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Could not parse last logout time.");
        }
    }

    // ------------------------------------------------------------------------
    // 💾 Save logout time on quit / suspend
    // ------------------------------------------------------------------------
    void OnApplicationQuit() => SaveLogoutTime();
    void OnApplicationPause(bool paused)
    {
        if (paused) SaveLogoutTime();
    }

    void SaveLogoutTime()
    {
        string now = DateTime.UtcNow.ToString("O");
        PlayerPrefs.SetString(LogoutKey, now);
        PlayerPrefs.Save();
        Debug.Log($"📦 Saved logout time: {now}");
    }
}
