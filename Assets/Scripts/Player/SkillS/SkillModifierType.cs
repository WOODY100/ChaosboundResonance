public enum SkillModifierType
{
    // Offensive
    FlatDamage,
    PercentDamage,
    CriticalChance,
    CriticalDamageBonus,

    // Tempo
    CooldownPercent,
    TickRatePercent,

    // Coverage
    SpawnRadiusPercent,
    ImpactRadiusPercent,
    RangePercent,
    ExtraCount,
    DurationPercent,
    Penetration,
    ChainCount,

    // Special
    ApplyBurn,
    ApplyShock,
    ApplyPoison,
    ExplodeOnKill,
    SplitOnImpact,
    SpawnZoneOnHit
}