using UnityEngine;

public enum SkillRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}

public enum SkillEffectType
{
    FlatPower,
    FlatHealth,
    FlatSpeed,
    PercentPower,
    PercentHealth,
    PercentCreditsGain,
    PercentXPGain
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "AFKIdle/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public string SkillName;
    [TextArea] public string Description;
    public SkillRarity Rarity = SkillRarity.Common;
    public Sprite Icon;

    [Header("Effect")]
    public SkillEffectType EffectType = SkillEffectType.FlatPower;
    public float EffectValue = 10f;
    public bool AffectsCompanions = false;

    [Header("Scaling")]
    public AnimationCurve LevelGrowth = AnimationCurve.Linear(1, 1, 50, 3);
}
