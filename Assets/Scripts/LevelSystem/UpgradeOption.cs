public class UpgradeOption
{
    public UpgradeType Type;

    // Para NewSkill y ReplaceSkill
    public SkillDefinition SkillDefinition;

    // Para SkillModifier
    public SkillModifierDefinition ModifierDefinition;

    // Slot objetivo (para modifier o replace)
    public int TargetSlotIndex;

    // Para buffs simples (ej: +10% daño global)
    public float Value;
}