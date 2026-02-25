using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Global Upgrade")]
public class GlobalUpgradeDefinition : ScriptableObject
{
    public string DisplayName;
    [TextArea] public string Description;

    public SkillRarity Rarity;

    public StatType TargetStat;
    public ModifierType ModifierType;
    public float Value;

    public Sprite Icon;
}