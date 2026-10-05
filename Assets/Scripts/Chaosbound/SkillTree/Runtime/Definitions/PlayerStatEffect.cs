using System;

[Serializable]
public sealed class PlayerStatEffect : SkillTreeEffect
{
    public StatType StatType;
    public ModifierType ModifierType;
    public float Value;
}