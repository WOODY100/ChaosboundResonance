using UnityEngine;

[System.Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType EffectType;

    // ===== Skill MaximumTarget =====
    public int TargetSlotIndex = -1;

    // ===== Skill Modifier =====
    public SkillModifierDefinition SkillModifier;

    // ===== Global Modifier =====
    public GlobalUpgradeDefinition GlobalDefinition;
    public StatType TargetStat;
    public ModifierType ModifierType;
    public float Value;
}