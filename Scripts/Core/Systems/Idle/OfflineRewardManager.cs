using UnityEngine;
using System;

public class OfflineRewardManager : MonoBehaviour
{
    public static OfflineRewardManager Instance;

    [Header("Settings")]
    public float MaxOfflineHours = 12f;
    public bool AutoGrantOnStart = true;

    private const string LAST_LOGOUT_KEY = "LastLogoutTime";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (AutoGrantOnStart)
            CheckOfflineRewards();
    }

    private void OnApplicationQuit()
    {
        SaveLogoutTime();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveLogoutTime();
    }

    public void SaveLogoutTime()
    {
        string timeStr = DateTime.UtcNow.ToString("O");
        PlayerPrefs.SetString(LAST_LOGOUT_KEY, timeStr);
        PlayerPrefs.Save();
        Debug.Log($"💾 Saved logout time: {timeStr}");
    }

    public void CheckOfflineRewards()
    {
        if (!PlayerPrefs.HasKey(LAST_LOGOUT_KEY))
        {
            Debug.Log("No previous logout time found. Skipping offline reward check.");
            return;
        }

        string stored = PlayerPrefs.GetString(LAST_LOGOUT_KEY);
        if (!DateTime.TryParse(stored, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastLogout))
        {
            Debug.LogWarning("Invalid stored logout time.");
            return;
        }

        double hours = (DateTime.UtcNow - lastLogout).TotalHours;
        if (hours <= 0.1)
        {
            Debug.Log("⏱️ Offline duration too short for rewards.");
            return;
        }

        float cappedHours = Mathf.Min((float)hours, MaxOfflineHours);

        Debug.Log($"⌛ Offline duration: {hours:F2}h (granting rewards for {cappedHours:F2}h).");

        var player = PlayerManager.Instance;
        if (player != null)
        {
            player.GrantOfflineRewards(cappedHours);
            Debug.Log($"🎁 Offline rewards granted → +XP & +Credits for {cappedHours:F1}h AFK.");
        }
        else
        {
            Debug.LogWarning("⚠️ No PlayerManager found; cannot apply offline rewards.");
        }
    }
}
