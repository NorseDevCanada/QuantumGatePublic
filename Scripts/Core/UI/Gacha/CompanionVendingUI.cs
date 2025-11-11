using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionVendingUI : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public CompanionSummonManager summonManager;

    [Header("UI Elements")]
    public TextMeshProUGUI ticketCountText;
    public Button summon15Button;
    public Button summon35Button;
    public Transform resultContainer;
    public GameObject resultItemPrefab;

    [Header("Visual Effects (Optional)")]
    public Animator vendingAnimator;
    public AudioSource summonSound;
    public Gradient rarityColorGradient;

    private void Start()
    {
        if (playerManager == null) playerManager = PlayerManager.Instance;
        if (summonManager == null) summonManager = CompanionSummonManager.Instance;

        if (summon15Button != null)
            summon15Button.onClick.AddListener(() => OnSummonButton(15));

        if (summon35Button != null)
            summon35Button.onClick.AddListener(() => OnSummonButton(35));

        RefreshTicketCount();
    }

    private void OnSummonButton(int count)
    {
        if (playerManager == null || summonManager == null) return;

        List<CompanionInstance> results = null;

        if (count == 15)
            results = summonManager.Summon15();
        else if (count == 35)
            results = summonManager.Summon35();

        if (results == null || results.Count == 0)
        {
            ShowMessage("❌ Not enough Companion Tickets!");
            return;
        }

        if (vendingAnimator != null)
            vendingAnimator.SetTrigger("Summon");

        if (summonSound != null)
            summonSound.Play();

        DisplayResults(results);
        RefreshTicketCount();
    }

    private void RefreshTicketCount()
    {
        if (ticketCountText != null && playerManager != null)
            ticketCountText.text = $"🎟️ Tickets: {playerManager.CompanionTickets}";
    }

    private void DisplayResults(List<CompanionInstance> companions)
    {
        foreach (Transform child in resultContainer)
            Destroy(child.gameObject);

        foreach (var comp in companions)
        {
            GameObject go = Instantiate(resultItemPrefab, resultContainer);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                Color rarityColor = GetRarityColor(comp.Rarity);
                text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(rarityColor)}>{comp.Data.CompanionName}</color> ({comp.Rarity})";
            }
        }
    }

    private Color GetRarityColor(CompanionRarity rarity)
    {
        switch (rarity)
        {
            case CompanionRarity.Common: return Color.gray;
            case CompanionRarity.Rare: return new Color(0.3f, 0.6f, 1f);
            case CompanionRarity.Epic: return new Color(0.7f, 0, 1f);
            case CompanionRarity.Legendary: return new Color(1f, 0.5f, 0.1f);
            case CompanionRarity.Mythic: return new Color(1f, 0.15f, 0.15f);
            default: return Color.white;
        }
    }

    private void ShowMessage(string msg)
    {
        if (resultContainer.childCount > 0)
        {
            foreach (Transform child in resultContainer)
                Destroy(child.gameObject);
        }

        GameObject go = Instantiate(resultItemPrefab, resultContainer);
        var text = go.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = msg;
    }
}
