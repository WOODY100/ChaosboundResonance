using UnityEngine;

[System.Serializable]
public struct ModifierEntry
{
    public SkillModifierType Type;
    public float Value;
}

[CreateAssetMenu(menuName = "Skills/Skill Modifier")]
public class SkillModifierDefinition : ScriptableObject
{
    [Header("UI")]
    public string DisplayName;

    [TextArea]
    public string Description;

    public Sprite Icon;

    public SkillRarity Rarity;

    [Header("Single Modifier (Legacy)")]
    public SkillModifierType ModifierType;
    public float Value;

    [Header("Multi Modifier (New)")]
    public ModifierEntry[] Modifiers;

    public bool IsStackable = true;
}