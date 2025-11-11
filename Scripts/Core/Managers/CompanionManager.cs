// Scripts/Companions/CompanionManager.cs
using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    [Header("Owned / Equipped")]
    public List<CompanionInstance> OwnedCompanions = new List<CompanionInstance>();
    public List<CompanionInstance> EquippedCompanions = new List<CompanionInstance>();
    public int MaxEquipped = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // find by ID (so dupes level the same one)
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

        // give merge XP
        int dupeXP = RarityCurve.GetDupeXP(data.Rarity, owned.Level);
        owned.AddXP(dupeXP);
        Debug.Log($"🐾 Dupe → +{dupeXP} XP to {owned.Data.CompanionName} (Lv {owned.Level})");
    }

    public void EquipCompanion(CompanionInstance inst)
    {
        if (inst == null) return;
        if (EquippedCompanions.Contains(inst)) return;

        if (EquippedCompanions.Count >= MaxEquipped)
        {
            Debug.Log("⚠️ Max equipped companions reached.");
            return;
        }

        EquippedCompanions.Add(inst);
        PlayerManager.Instance?.RecalculateStats();
    }

    public void UnequipCompanion(CompanionInstance inst)
    {
        if (EquippedCompanions.Remove(inst))
            PlayerManager.Instance?.RecalculateStats();
    }

    public int GetTotalEquippedCP()
    {
        int total = 0;
        foreach (var c in EquippedCompanions)
            total += c.GetCombatPower();
        return total;
    }
}
