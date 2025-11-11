using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AFKSessionSummary : MonoBehaviour
{
    [Header("References")]
    public GameObject summaryPanel;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI efficiencyText;
    public Button closeButton;

    private float startTime;
    private int startXP;
    private float startCredits;
    private int startKills;
    private bool sessionActive = false;

    private PlayerManager player;
    private EnemyManager enemy;

    void Start()
    {
        player = PlayerManager.Instance;
        enemy = EnemyManager.Instance;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSummary);

        HideSummary();
    }

    public void BeginSession()
    {
        if (sessionActive) return;
        sessionActive = true;

        startTime = Time.time;
        startXP = player.CurrentXP;
        startCredits = player.TotalCredits;
        startKills = enemy.TotalEnemiesDefeated;

        Debug.Log("🕓 AFK Session started.");
    }

    public void EndSession()
    {
        if (!sessionActive) return;
        sessionActive = false;

        float duration = Time.time - startTime;
        int gainedXP = player.CurrentXP - startXP;
        float gainedCredits = player.TotalCredits - startCredits;
        int kills = enemy.TotalEnemiesDefeated - startKills;

        float hours = duration / 3600f;
        float killsPerHour = (kills / Mathf.Max(hours, 0.001f));
        float creditsPerHour = (gainedCredits / Mathf.Max(hours, 0.001f));

        ShowSummary(duration, gainedXP, gainedCredits, kills, killsPerHour, creditsPerHour);

        Debug.Log("📊 AFK Session ended and summary displayed.");
    }

    private void ShowSummary(float duration, int xp, float credits, int kills, float killsPerHour, float creditsPerHour)
    {
        if (summaryPanel != null)
            summaryPanel.SetActive(true);

        TimeSpan t = TimeSpan.FromSeconds(duration);
        string timeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

        headerText.text = "⚔️ AFK Battle Summary";
        durationText.text = $"⏱️ Duration: {timeFormatted}";
        killsText.text = $"💀 Enemies Defeated: {kills:N0}";
        xpText.text = $"⭐ XP Gained: {xp:N0}";
        creditsText.text = $"💰 Credits Earned: {credits:N0}";
        efficiencyText.text = $"⚡ Kills/hr: {killsPerHour:N0}  |  Cr/hr: {creditsPerHour:N0:N0}";

        Debug.Log($"📊 AFK Summary → {kills} kills, {xp} XP, {credits:N0} Cr over {timeFormatted}");
    }

    public void HideSummary()
    {
        if (summaryPanel != null)
            summaryPanel.SetActive(false);
    }

    private void CloseSummary()
    {
        HideSummary();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused && sessionActive)
        {
            Debug.Log("⏸️ Game paused — ending AFK session safely.");
            EndSession();
        }
    }

    void OnApplicationQuit()
    {
        if (sessionActive)
        {
            Debug.Log("🛑 Game quitting — ending AFK session safely.");
            EndSession();
        }
    }

    [Serializable]
    public struct AFKSessionStartData
    {
        public string playerId;
        public DateTime startTimestamp;
        public int stage;
    }

    [Serializable]
    public struct AFKSessionEndData
    {
        public string playerId;
        public double durationSeconds;
        public int totalKills;
        public int totalXP;
        public float totalCredits;
        public DateTime endTimestamp;
    }
}
