public enum StatType
{
    // Core
    HP, ATK, DEF, ATKSPD,

    // Offensive
    CritRate, CritDMG, CritRES,
    SkillDMG, SkillCrit, SkillDMGRES,
    Combo, ComboMultiplier, ComboDMGRES,
    BossDMG, BossDMGRES,
    Pierce, IgnorePierce, Block, IgnoreBlock,

    // Defensive
    HPBonus, ATKBonus, DEFBonus, DMGRES,
    Evasion, IgnoreEvasion,
    CounterMultiplier, CounterDMGRES,

    // Support
    HealingRate, HealingAmount,

    // Control / Special
    Stun, IgnoreStun, Launch, IgnoreLaunch,
    Regen, IgnoreRegen,
    PalInspire, IgnorePalInspire, PalRES, IgnorePalRES
}
