using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Evolution")]
public class SkillEvolutionDefinition : ScriptableObject
{
    // =========================
    // UI
    // =========================

    [Header("UI")]
    public string DisplayName;

    [TextArea]
    public string Description;

    public Sprite Icon;

    public SkillRarity Rarity;

    // =========================
    // RESULT
    // =========================

    [Header("Evolution Result")]
    [Tooltip("SkillDefinition that replaces the current skill when this evolution is selected.")]
    public SkillDefinition ResultingSkill;
}