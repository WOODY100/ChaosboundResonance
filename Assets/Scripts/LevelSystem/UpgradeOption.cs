using System.Collections.Generic;

[System.Serializable]
public class UpgradeOption
{
    public SkillDefinition SkillDefinition;
    public SkillModifierDefinition ModifierDefinition;
    public int TargetSlotIndex;

    public readonly List<UpgradeEffect> Effects = new();
}