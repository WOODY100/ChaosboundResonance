using System.Collections.Generic;

[System.Serializable]
public class UpgradeOption
{
    public UpgradeType Type;

    public SkillDefinition SkillDefinition;
    public SkillModifierDefinition ModifierDefinition;
    public int TargetSlotIndex;

    public List<UpgradeEffect> Effects = new List<UpgradeEffect>();
}