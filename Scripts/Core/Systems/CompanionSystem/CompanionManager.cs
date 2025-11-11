using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    [Header("Player Companions")]
    public List<CompanionInstance> OwnedCompanions = new List<CompanionInstance>();
    public List<CompanionInstance> EquippedCompanions = new List<CompanionInstance>();

    [Header("Limits")]
    public int MaxEquipped = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------------------------------------------------------------
    // 🧩 Management
    // ------------------------------------------------------------------------
    public void AddCompanion(CompanionInstance newComp)
    {
        OwnedCompanions.Add(newComp);
        Debug.Log($"🐾 New Companion Acquired: {newComp.Data.CompanionName} ({newComp.Rarity})");
    }

    public void EquipCompanion(CompanionInstance comp)
    {
        if (EquippedCompanions.Count >= MaxEquipped)
        {
            Debug.Log("⚠️ Max equipped companions reached.");
            return;
        }

        if (!OwnedCompanions.Contains(comp))
        {
            Debug.LogWarning("Companion not owned!");
            return;
        }

        EquippedCompanions.Add(comp);
        Debug.Log($"🧬 Equipped Companion: {comp.Data.CompanionName}");
        PlayerManager.Instance?.RecalculateStats();
    }

    public void UnequipCompanion(CompanionInstance comp)
    {
        if (EquippedCompanions.Contains(comp))
        {
            EquippedCompanions.Remove(comp);
            PlayerManager.Instance?.RecalculateStats();
        }
    }

    public int GetTotalEquippedCP()
    {
        int total = 0;
        foreach (var comp in EquippedCompanions)
        {
            total += comp.GetCombatPower();
        }
        return total;
    }
}
