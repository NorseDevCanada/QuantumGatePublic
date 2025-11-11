using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AFKBattleHUD : MonoBehaviour
{
    [Header("References")]
    public EnemyManager enemyManager;
    public AFKBattleManager afkManager;
    public PlayerManager playerManager;

    [Header("UI Elements")]
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI killsPerHourText;
    public Slider stageProgressBar;

    [Header("Refresh Rate")]
    [Tooltip("How often (in seconds) to update displayed stats.")]
    public float refreshInterval = 0.5f;

    private int lastKills = 0;
    private float timer = 0f;
    private float timeElapsed = 0f;

    private int totalKillsSinceStart = 0;
    private int xpAtStart = 0;
    private float creditsAtStart = 0f;

    void Start()
    {
        if (enemyManager == null) enemyManager = EnemyManager.Instance;
        if (playerManager == null) playerManager = PlayerManager.Instance;
        if (afkManager == null) afkManager = AFKBattleManager.Instance;

        xpAtStart = playerManager.CurrentXP;
        creditsAtStart = playerManager.TotalCredits;

        UpdateUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timer >= refreshInterval)
        {
            timer = 0f;
            UpdateUI();
        }

        // Track kills for rate stats
        if (enemyManager.EnemiesDefeated != lastKills)
        {
            int diff = enemyManager.EnemiesDefeated - lastKills;
            totalKillsSinceStart += Mathf.Max(0, diff);
            lastKills = enemyManager.EnemiesDefeated;
        }
    }

    // ------------------------------------------------------------------------
    // 🧾 UI Updates
    // ------------------------------------------------------------------------
    private void UpdateUI()
    {
        if (enemyManager == null || playerManager == null) return;

        int stage = enemyManager.CurrentStage;
        int defeated = enemyManager.EnemiesDefeated;
        int required = enemyManager.EnemiesPerStage;

        // Stage progress bar
        if (stageProgressBar != null)
            stageProgressBar.value = (float)defeated / required;

        // Text values
        stageText.text = $"🌍 Stage {stage}";
        killsText.text = $"💀 {defeated}/{required} Kills";

        int xpGained = playerManager.CurrentXP - xpAtStart;
        float creditsGained = playerManager.TotalCredits - creditsAtStart;
        xpText.text = $"⭐ XP: +{xpGained:N0}";
        creditsText.text = $"💰 Credits: +{creditsGained:N0}";

        // Calculate kills/hour
        float killsPerHour = (totalKillsSinceStart / Mathf.Max(timeElapsed, 1f)) * 3600f;
        killsPerHourText.text = $"⚔️ Kills/hr: {killsPerHour:N0}";
    }
}
