using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [Header("Skill Management")]
    public List<SkillInstance> OwnedSkills = new List<SkillInstance>();
    public List<SkillInstance> EquippedSkills = new List<SkillInstance>();
    public int MaxEquippedSkills = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Skill Lookup
    public SkillInstance FindOwnedByName(string skillName)
    {
        return OwnedSkills.Find(s => s.Data != null && s.Data.SkillName == skillName);
    }

    // Adding / Duplicates
    public void AddNewSkill(SkillData data)
    {
        if (data == null)
        {
            Debug.LogWarning("⚠️ Tried to add a null SkillData.");
            return;
        }

        var inst = new SkillInstance
        {
            Data = data,
            Level = 1,
            CurrentXP = 0,
            XPToNextLevel = RarityCurve.GetXPRequiredForLevel(1)
        };

        OwnedSkills.Add(inst);
        Debug.Log($"📜 New skill obtained: {data.SkillName}");
    }

    public void AddDupeSkill(SkillData data)
    {
        if (data == null)
        {
            Debug.LogWarning("⚠️ Tried to add a duplicate of a null skill.");
            return;
        }

        var owned = FindOwnedByName(data.SkillName);
        if (owned == null)
        {
            AddNewSkill(data);
            return;
        }

        int dupeXP = RarityCurve.GetDupeXP(data.Rarity, owned.Level);
        owned.AddXP(dupeXP);
        Debug.Log($"📜 Dupe skill → +{dupeXP} XP to {owned.Data.SkillName} (Lv {owned.Level})");
    }

    // Equip / Unequip
    public void EquipSkill(SkillInstance skill)
    {
        if (skill == null) return;
        if (EquippedSkills.Contains(skill)) return;

        if (EquippedSkills.Count >= MaxEquippedSkills)
        {
            Debug.Log("⚠️ Max equipped skills reached.");
            return;
        }

        EquippedSkills.Add(skill);
        PlayerManager.Instance?.RecalculateStats();
    }

    public void UnequipSkill(SkillInstance skill)
    {
        if (EquippedSkills.Remove(skill))
            PlayerManager.Instance?.RecalculateStats();
    }

    // Apply Skill Effects
    public void ApplySkillBonusesToPlayer(PlayerManager player)
    {
        if (EquippedSkills == null || EquippedSkills.Count == 0 || player == null)
            return;

        foreach (var skill in EquippedSkills)
        {
            if (skill == null || skill.Data == null)
                continue;

            float value = skill.GetEffectiveValue();
            float rarityMult = SkillScaling.GetRarityMultiplier(skill.Rarity);
            value *= rarityMult;

            switch (skill.Data.EffectType)
            {
                case SkillEffectType.FlatPower:
                    player.Stats.TotalPower += Mathf.RoundToInt(value);
                    break;
                case SkillEffectType.FlatHealth:
                    player.Stats.TotalHealth += Mathf.RoundToInt(value);
                    break;
                case SkillEffectType.FlatSpeed:
                    player.Stats.TotalSpeed += Mathf.RoundToInt(value);
                    break;
                case SkillEffectType.PercentPower:
                    player.Stats.TotalPower = Mathf.RoundToInt(player.Stats.TotalPower * (1f + value / 100f));
                    break;
                case SkillEffectType.PercentHealth:
                    player.Stats.TotalHealth = Mathf.RoundToInt(player.Stats.TotalHealth * (1f + value / 100f));
                    break;
                case SkillEffectType.PercentCreditsGain:
                    // handled in Stage / Economy systems
                    break;
                case SkillEffectType.PercentXPGain:
                    // handled in Stage / XP systems
                    break;
            }
        }
    }

    // Bonus Accessors
    public float GetXPBonusPercent()
    {
        float bonus = 0f;
        foreach (var s in EquippedSkills)
        {
            if (s != null && s.Data != null && s.Data.EffectType == SkillEffectType.PercentXPGain)
                bonus += s.GetEffectiveValue();
        }
        return bonus;
    }

    public float GetCreditBonusPercent()
    {
        float bonus = 0f;
        foreach (var s in EquippedSkills)
        {
            if (s != null && s.Data != null && s.Data.EffectType == SkillEffectType.PercentCreditsGain)
                bonus += s.GetEffectiveValue();
        }
        return bonus;
    }
}
