using System;
using UnityEngine;

[Serializable]
public sealed class ChoiceOption
{
    public string OptionId;
    public string DisplayName;
    public string Description;
    public UnityEngine.Sprite Icon;

    [SerializeReference]
    public SkillTreeEffect[] Effects;
}