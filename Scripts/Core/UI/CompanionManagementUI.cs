using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionManagementUI : MonoBehaviour
{
    [Header("References")]
    public CompanionManager companionManager;
    public PlayerManager playerManager;

    [Header("UI Elements")]
    public Transform ownedListParent;              // parent container for owned companion entries
    public Transform equippedListParent;           // parent container for equipped companion entries
    public GameObject companionListItemPrefab;     // simple prefab with text and buttons
    public TextMeshProUGUI totalCPText;            // displays total equipped CP

    private void Start()
    {
        if (companionManager == null) companionManager = CompanionManager.Instance;
        if (playerManager == null) playerManager = PlayerManager.Instance;

        RefreshUI();
    }

    // ------------------------------------------------------------------------
    // 🔄 UI Refresh
    // ------------------------------------------------------------------------
    public void RefreshUI()
    {
        if (companionManager == null) return;

        ClearList(ownedListParent);
        ClearList(equippedListParent);

        // --- Owned List ---
        foreach (var comp in companionManager.OwnedCompanions)
        {
            GameObject go = Instantiate(companionListItemPrefab, ownedListParent);
            var item = go.GetComponent<CompanionListItemUI>();
            if (item != null)
                item.Setup(comp, this, isEquippedList: false);
        }

        // --- Equipped List ---
        foreach (var comp in companionManager.EquippedCompanions)
        {
            GameObject go = Instantiate(companionListItemPrefab, equippedListParent);
            var item = go.GetComponent<CompanionListItemUI>();
            if (item != null)
                item.Setup(comp, this, isEquippedList: true);
        }

        // --- Total CP ---
        int totalCP = companionManager.GetTotalEquippedCP();
        totalCPText.text = $"Equipped Companions CP: {totalCP:N0}";
    }

    private void ClearList(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    // ------------------------------------------------------------------------
    // ⚙️ Button Events (called by list item buttons)
    // ------------------------------------------------------------------------
    public void OnEquip(CompanionInstance comp)
    {
        companionManager.EquipCompanion(comp);
        RefreshUI();
    }

    public void OnUnequip(CompanionInstance comp)
    {
        companionManager.UnequipCompanion(comp);
        RefreshUI();
    }
}
