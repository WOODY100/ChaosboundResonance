using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Evolution")]
public class SkillEvolutionDefinition : ScriptableObject
{
    [Header("UI")]
    public string DisplayName;

    [TextArea]
    public string Description;

    public Sprite Icon;
    public SkillRarity Rarity;

    [Header("Stat Bonuses")]
    public float BonusFlatDamage;
    public float BonusPercentDamage;
    public float BonusSpawnRadiusPercent;
    public float BonusImpactRadiusPercent;
    public float BonusCooldownPercent;
    public float BonusDurationPercent;

    public int BonusExtraCount;
    public int BonusPenetration;
    public int BonusChainCount;

    [Header("Behavior Flags")]
    public bool GrantsExplosion;
    public bool GrantsSplit;
    public bool GrantsChaining;
    public bool GrantsExplosionOnHit;
}