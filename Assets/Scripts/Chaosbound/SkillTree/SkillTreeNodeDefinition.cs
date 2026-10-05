using Chaosbound.SkillTree;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkillTreeNodeDefinition",
    menuName = "Chaosbound/Skill Tree/Node Definition"
)]
public sealed class SkillTreeNodeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string NodeId;

    [Header("Presentation")]
    public string DisplayName;
    [TextArea(2, 5)]
    public string Description;
    public Sprite Icon;

    [Header("Structure")]
    public SkillTreeBranch Branch;
    public SkillTreeNodeType NodeType;

    [Header("Cost")]
    [Min(0)]
    public int Cost;

    [Header("Requirement")]
    [SerializeReference]
    public NodeRequirement Requirement;

    [Header("Effects")]
    [SerializeReference]
    public SkillTreeEffect[] Effects;

    [Header("Choice")]
    public ChoiceOption[] ChoiceOptions;
}