using System.Collections.Generic;

[System.Serializable]
public class UpgradeOption
{
    public SkillDefinition SkillDefinition;
    public SkillModifierDefinition ModifierDefinition;
    public int TargetSlotIndex;

#if UNITY_EDITOR
    [System.NonSerialized]
    public string AnalyticsOptionId;
#endif

    public readonly List<UpgradeEffect> Effects = new();
}