using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Evolution")]
public class SkillEvolutionDefinition : ScriptableObject
{
    public bool GrantsAnySpecialBehavior =>
        GrantsExplosion ||
        GrantsSplit ||
        GrantsChaining ||
        GrantsExplosionOnHit;

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
    // STAT BONUSES
    // =========================

    [Header("Stat Bonuses")]

    [Tooltip("Flat damage added before multipliers.")]
    public float BonusFlatDamage;

    [Tooltip("Percentage damage bonus (0.20 = +20%).")]
    public float BonusPercentDamage;

    [Tooltip("Spawn radius bonus.")]
    public float BonusSpawnRadiusPercent;

    [Tooltip("Impact radius bonus.")]
    public float BonusImpactRadiusPercent;

    [Tooltip("Cooldown reduction percentage.")]
    public float BonusCooldownPercent;

    [Tooltip("Duration increase percentage.")]
    public float BonusDurationPercent;

    [Tooltip("Additional projectiles or instances.")]
    public int BonusExtraCount;

    [Tooltip("Additional penetration count.")]
    public int BonusPenetration;

    [Tooltip("Additional chain count.")]
    public int BonusChainCount;

    // =========================
    // SPECIAL BEHAVIOR
    // =========================

    [Header("Behavior")]

    public bool GrantsExplosion;

    public bool GrantsSplit;

    public bool GrantsChaining;

    public bool GrantsExplosionOnHit;

    // =========================
    // VALIDATION
    // =========================

    private void OnValidate()
    {
        BonusExtraCount = Mathf.Max(0, BonusExtraCount);
        BonusPenetration = Mathf.Max(0, BonusPenetration);
        BonusChainCount = Mathf.Max(0, BonusChainCount);

        BonusCooldownPercent = Mathf.Max(0f, BonusCooldownPercent);
    }
}