using System;

[Serializable]
public sealed class GlobalSkillModifierEffect : SkillTreeEffect
{
    public SkillModifierType ModifierType;
    public float Value;
}