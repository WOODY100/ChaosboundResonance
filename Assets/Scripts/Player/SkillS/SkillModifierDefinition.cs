using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Modifier")]
public class SkillModifierDefinition : ScriptableObject
{
    [Header("UI")]
    public string DisplayName;

    [TextArea]
    public string Description;

    public Sprite Icon;

    public SkillRarity Rarity;

    [Header("Modifier")]
    public SkillModifierType ModifierType;
    public float Value;

    public bool IsStackable = true;
}