using UnityEngine;

public enum CompanionRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}

[CreateAssetMenu(fileName = "NewCompanion", menuName = "AFKIdle/Companion Data")]
public class CompanionData : ScriptableObject
{
    [Header("Unique ID")]
    [Tooltip("Used internally to identify this companion for dupes & saves.")]
    public string CompanionID = System.Guid.NewGuid().ToString();

    [Header("Basic Info")]
    public string CompanionName;
    [TextArea] public string Description;
    public Sprite Icon;
    public CompanionRarity Rarity = CompanionRarity.Common;

    [Header("Base Stats")]
    public int BasePower = 50;
    public int BaseHealth = 200;
    public float AttackSpeed = 1.5f;

    [Header("Passive Bonus (applied to player)")]
    [Tooltip("Percent-based boost to player combat power.")]
    [Range(0f, 0.5f)]
    public float PlayerCPBonusPercent = 0.05f;

    [Header("Scaling")]
    public AnimationCurve PowerGrowth = AnimationCurve.EaseInOut(1, 1, 100, 5);
    public AnimationCurve HealthGrowth = AnimationCurve.EaseInOut(1, 1, 100, 8);
}
