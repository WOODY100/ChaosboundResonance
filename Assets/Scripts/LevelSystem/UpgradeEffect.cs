using UnityEngine;

[System.Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType EffectType;

    // ===== Skill Target =====
    public int TargetSlotIndex;

    // ===== Skill Modifier =====
    public SkillModifierDefinition SkillModifier;

    // ===== Skill Evolution =====
    public SkillEvolutionDefinition SkillEvolution;

    // ===== Global Modifier =====
    public GlobalUpgradeDefinition GlobalDefinition;
    public StatType TargetStat;
    public ModifierType ModifierType;
    public float Value;
}