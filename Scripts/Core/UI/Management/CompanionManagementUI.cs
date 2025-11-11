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
    public Transform ownedListParent;
    public Transform equippedListParent;
    public GameObject companionListItemPrefab;
    public TextMeshProUGUI totalCPText;

    private void Start()
    {
        if (companionManager == null) companionManager = CompanionManager.Instance;
        if (playerManager == null) playerManager = PlayerManager.Instance;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (companionManager == null) return;

        ClearList(ownedListParent);
        ClearList(equippedListParent);

        foreach (var comp in companionManager.OwnedCompanions)
        {
            GameObject go = Instantiate(companionListItemPrefab, ownedListParent);
            var item = go.GetComponent<CompanionListItemUI>();
            if (item != null)
                item.Setup(comp, this, isEquippedList: false);
        }

        foreach (var comp in companionManager.EquippedCompanions)
        {
            GameObject go = Instantiate(companionListItemPrefab, equippedListParent);
            var item = go.GetComponent<CompanionListItemUI>();
            if (item != null)
                item.Setup(comp, this, isEquippedList: true);
        }

        int totalCP = companionManager.GetTotalEquippedCP();
        totalCPText.text = $"Equipped Companions CP: {totalCP:N0}";
    }

    private void ClearList(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

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
