using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuantumGateUIDynamic : MonoBehaviour
{
    [Header("References")]
    public QuantumGateManager gateManager;
    public PlayerManager playerManager;

    [Header("UI Elements")]
    public TextMeshProUGUI gateLevelText;
    public TextMeshProUGUI shardCountText;
    public TextMeshProUGUI pullsPerActivationText;
    public TextMeshProUGUI resultText;
    public Button activateGateButton;

    [Header("Visuals (Optional)")]
    public Image rarityGlowImage;
    public Gradient rarityColorGradient;
    public float itemRevealDelay = 0.15f;
    public float glowFlashDuration = 0.25f;

    private Coroutine displayCoroutine;

    private void Start()
    {
        if (gateManager == null) gateManager = QuantumGateManager.Instance;
        if (playerManager == null) playerManager = PlayerManager.Instance;

        if (activateGateButton != null)
            activateGateButton.onClick.AddListener(OnActivateGateClicked);

        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    public void OnActivateGateClicked()
    {
        if (playerManager == null || gateManager == null)
            return;

        if (!playerManager.SpendQuantumShard())
        {
            resultText.text = "<color=red>❌ Not enough Quantum Shards!</color>";
            return;
        }

        List<Gear> results = gateManager.DoGateActivation();
        if (results == null || results.Count == 0)
        {
            resultText.text = "⚠️ No gear pulled.";
            return;
        }

        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        displayCoroutine = StartCoroutine(AnimateResults(results));
    }

    public void RefreshUI()
    {
        if (gateManager == null || playerManager == null)
            return;

        gateLevelText.text = $"Gate Level: {gateManager.CurrentGateLevel}";
        shardCountText.text = $"Quantum Shards: {playerManager.QuantumShards:N0}";
        pullsPerActivationText.text = $"Pulls per Activation: {gateManager.GetPullsForLevel(gateManager.CurrentGateLevel)}";
    }

    private IEnumerator AnimateResults(List<Gear> pulledGears)
    {
        resultText.text = "🎁 <b>Pull Results</b>:\n";
        if (rarityGlowImage != null)
            rarityGlowImage.color = Color.clear;

        yield return new WaitForSeconds(0.25f);

        int index = 0;
        foreach (var gear in pulledGears)
        {
            Color rarityColor = GetRarityColor(gear.Rarity);
            string gearLine = $"<color=#{ColorUtility.ToHtmlStringRGB(rarityColor)}>{gear.GearName}</color> ({gear.Rarity})\n";

            resultText.text += gearLine;

            if (rarityGlowImage != null && rarityColorGradient != null)
            {
                Color glowColor = rarityColorGradient.Evaluate(Mathf.InverseLerp(0, 10, (int)gear.Rarity));
                StartCoroutine(FlashGlow(glowColor));
            }

            index++;
            yield return new WaitForSeconds(itemRevealDelay);
        }

        resultText.text += "\n✨ All pulls revealed!";
    }

    private IEnumerator FlashGlow(Color c)
    {
        float timer = 0f;
        while (timer < glowFlashDuration)
        {
            float t = Mathf.PingPong(timer * 4f, 1f);
            rarityGlowImage.color = Color.Lerp(Color.clear, c, t);
            timer += Time.deltaTime;
            yield return null;
        }
        rarityGlowImage.color = Color.clear;
    }

    private Color GetRarityColor(GearRarity rarity)
    {
        switch (rarity)
        {
            case GearRarity.Normal: return new Color(0.6f, 0.6f, 0.6f);
            case GearRarity.Unique: return new Color(0.75f, 0.75f, 0.75f);
            case GearRarity.Well: return new Color(0.55f, 0.85f, 1f);
            case GearRarity.Rare: return new Color(0.4f, 0.6f, 1f);
            case GearRarity.Mythic: return new Color(0.65f, 0.25f, 1f);
            case GearRarity.Epic: return new Color(0.9f, 0.3f, 0.8f);
            case GearRarity.Legendary: return new Color(1f, 0.65f, 0f);
            case GearRarity.Immortal: return new Color(1f, 0.4f, 0.4f);
            case GearRarity.Supreme: return new Color(1f, 0.5f, 0.7f);
            case GearRarity.Radiant: return new Color(1f, 0.9f, 0.3f);
            case GearRarity.Eternal: return new Color(0.95f, 0.95f, 1f);
            default: return Color.white;
        }
    }
}
