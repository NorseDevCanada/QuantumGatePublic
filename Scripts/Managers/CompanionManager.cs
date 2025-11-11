using System.Collections.Generic;
using UnityEngine;

// Consolidated CompanionManager: merges logic from Systems/CompanionSystem and Managers copies.
public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    [Header("Owned / Equipped")]
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
    // 🧩 Find / Add helpers
    // ------------------------------------------------------------------------
    // find by ID (so dupes level the same one)
    public CompanionInstance FindOwnedByID(string id)
    {
        return OwnedCompanions.Find(c => c.Data != null && c.Data.CompanionID == id);
    }

    public CompanionInstance AddNewCompanion(CompanionData data)
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
        return inst;
    }

    // Older callers may provide a CompanionInstance directly
    public void AddCompanion(CompanionInstance newComp)
    {
        OwnedCompanions.Add(newComp);
        Debug.Log($"🐾 New Companion Acquired: {newComp.Data.CompanionName} ({newComp.Rarity})");
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

    // ------------------------------------------------------------------------
    // 🧭 Equip / Unequip
    // ------------------------------------------------------------------------
    public void EquipCompanion(CompanionInstance inst)
    {
        if (inst == null) return;
        if (EquippedCompanions.Contains(inst)) return;

        if (EquippedCompanions.Count >= MaxEquipped)
        {
            Debug.Log("⚠️ Max equipped companions reached.");
            return;
        }

        if (!OwnedCompanions.Contains(inst)) OwnedCompanions.Add(inst);

        EquippedCompanions.Add(inst);
        Debug.Log($"🧬 Equipped Companion: {inst.Data.CompanionName}");
        PlayerManager.Instance?.RecalculateStats();
    }

    public void UnequipCompanion(CompanionInstance inst)
    {
        if (EquippedCompanions.Contains(inst))
        {
            EquippedCompanions.Remove(inst);
            PlayerManager.Instance?.RecalculateStats();
        }
    }

    // ------------------------------------------------------------------------
    // 📊 Power helpers
    // ------------------------------------------------------------------------
    public int GetTotalEquippedCP()
    {
        int total = 0;
        foreach (var c in EquippedCompanions)
            total += c.GetCombatPower();
        return total;
    }

    // Backwards-compatible alias used by older callers
    public int GetTotalCompanionPower() => GetTotalEquippedCP();
}
