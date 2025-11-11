using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UNIFIED CompanionManager - SINGLE SOURCE OF TRUTH
/// Consolidates all companion management logic from duplicate implementations
/// </summary>
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

    // Find by ID (so dupes level the same one)
    public CompanionInstance FindOwnedByID(string id)
    {
        return OwnedCompanions.Find(c => c.Data != null && c.Data.CompanionID == id);
    }

    public void AddNewCompanion(CompanionData data)
    {
        var inst = new CompanionInstance
        {
            Data = data,
            Level = 1,
            CurrentXP = 0,
            XPToNextLevel = RarityCurve.GetXPRequiredForLevel(1)
        };
        OwnedCompanions.Add(inst);
        Debug.Log($"🐾 New companion obtained: {data.CompanionName}");
    }

    public void AddDupeCompanion(CompanionData data)
    {
        var owned = FindOwnedByID(data.CompanionID);
        if (owned == null)
        {
            AddNewCompanion(data);
            return;
        }

        // Give merge XP
        int dupeXP = RarityCurve.GetDupeXP(data.Rarity, owned.Level);
        owned.AddXP(dupeXP);
        Debug.Log($"🐾 Dupe → +{dupeXP} XP to {owned.Data.CompanionName} (Lv {owned.Level})");
    }

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
        if (EquippedCompanions.Remove(comp))
            PlayerManager.Instance?.RecalculateStats();
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
