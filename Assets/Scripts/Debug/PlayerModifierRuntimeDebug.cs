using UnityEngine;

public sealed class PlayerModifierRuntimeDebug : MonoBehaviour
{
    [Header("Runtime Reference")]
    [SerializeField]
    private PlayerModifierSystem playerModifierSystem;

    [Header("Debug State")]
    [SerializeField]
    private bool captureBaselineOnStart = true;

    [SerializeField]
    private bool logChanges = true;

    [Header("Current Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;
    [SerializeField] private float maxHP;
    [SerializeField] private float hpRegen;
    [SerializeField] private float damageReduction;
    [SerializeField] private float shield;
    [SerializeField] private float pickupRadius;
    [SerializeField] private float luck;
    [SerializeField] private float xpGain;

    [Header("Baseline")]
    [SerializeField] private float baselineDamage;
    [SerializeField] private float baselineAttackSpeed;
    [SerializeField] private float baselineMovementSpeed;
    [SerializeField] private float baselineCritChance;
    [SerializeField] private float baselineCritDamage;
    [SerializeField] private float baselineMaxHP;
    [SerializeField] private float baselineHPRegen;
    [SerializeField] private float baselineDamageReduction;
    [SerializeField] private float baselineShield;
    [SerializeField] private float baselinePickupRadius;
    [SerializeField] private float baselineLuck;
    [SerializeField] private float baselineXPGain;

    private void Start()
    {
        if (playerModifierSystem == null)
        {
            playerModifierSystem =
                GetComponent<PlayerModifierSystem>();
        }

        if (playerModifierSystem == null)
        {
            Debug.LogError(
                "[Player Modifier Runtime Debug] " +
                "PlayerModifierSystem was not found.",
                this);

            enabled = false;
            return;
        }

        RefreshCurrentStats();

        if (captureBaselineOnStart)
        {
            CaptureBaseline();
        }
    }

    private void Update()
    {
        if (playerModifierSystem == null)
            return;

        RefreshCurrentStats();
    }

    [ContextMenu("Capture Baseline")]
    public void CaptureBaseline()
    {
        if (playerModifierSystem == null)
            return;

        baselineDamage =
            playerModifierSystem.GetStat(
                StatType.Damage);

        baselineAttackSpeed =
            playerModifierSystem.GetStat(
                StatType.AttackSpeed);

        baselineMovementSpeed =
            playerModifierSystem.GetStat(
                StatType.MovementSpeed);

        baselineCritChance =
            playerModifierSystem.GetStat(
                StatType.CritChance);

        baselineCritDamage =
            playerModifierSystem.GetStat(
                StatType.CritDamage);

        baselineMaxHP =
            playerModifierSystem.GetStat(
                StatType.MaxHP);

        baselineHPRegen =
            playerModifierSystem.GetStat(
                StatType.HPRegen);

        baselineDamageReduction =
            playerModifierSystem.GetStat(
                StatType.DamageReduction);

        baselineShield =
            playerModifierSystem.GetStat(
                StatType.Shield);

        baselinePickupRadius =
            playerModifierSystem.GetStat(
                StatType.PickupRadius);

        baselineLuck =
            playerModifierSystem.GetStat(
                StatType.Luck);

        baselineXPGain =
            playerModifierSystem.GetStat(
                StatType.XPGain);

        Debug.Log(
            "[Player Modifier Runtime Debug] " +
            "Baseline captured.",
            this);
    }

    [ContextMenu("Log Current Stats")]
    public void LogCurrentStats()
    {
        if (playerModifierSystem == null)
            return;

        RefreshCurrentStats();

        Debug.Log(
            "[Player Modifier Runtime Debug]\n" +
            $"Damage: {damage}\n" +
            $"Attack Speed: {attackSpeed}\n" +
            $"Movement Speed: {movementSpeed}\n" +
            $"Crit Chance: {critChance}\n" +
            $"Crit Damage: {critDamage}\n" +
            $"Max HP: {maxHP}\n" +
            $"HP Regen: {hpRegen}\n" +
            $"Damage Reduction: {damageReduction}\n" +
            $"Shield: {shield}\n" +
            $"Pickup Radius: {pickupRadius}\n" +
            $"Luck: {luck}\n" +
            $"XP Gain: {xpGain}",
            this);
    }

    private void RefreshCurrentStats()
    {
        damage =
            playerModifierSystem.GetStat(
                StatType.Damage);

        attackSpeed =
            playerModifierSystem.GetStat(
                StatType.AttackSpeed);

        movementSpeed =
            playerModifierSystem.GetStat(
                StatType.MovementSpeed);

        critChance =
            playerModifierSystem.GetStat(
                StatType.CritChance);

        critDamage =
            playerModifierSystem.GetStat(
                StatType.CritDamage);

        maxHP =
            playerModifierSystem.GetStat(
                StatType.MaxHP);

        hpRegen =
            playerModifierSystem.GetStat(
                StatType.HPRegen);

        damageReduction =
            playerModifierSystem.GetStat(
                StatType.DamageReduction);

        shield =
            playerModifierSystem.GetStat(
                StatType.Shield);

        pickupRadius =
            playerModifierSystem.GetStat(
                StatType.PickupRadius);

        luck =
            playerModifierSystem.GetStat(
                StatType.Luck);

        xpGain =
            playerModifierSystem.GetStat(
                StatType.XPGain);
    }
}