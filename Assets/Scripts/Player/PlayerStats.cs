using UnityEngine;
using System;

[RequireComponent(typeof(PlayerModifierSystem))]
public class PlayerStats : MonoBehaviour
{
    private PlayerModifierSystem modifierSystem;

    public float FinalDamage => modifierSystem.GetStat(StatType.Damage);
    public float FinalAttackSpeed => modifierSystem.GetStat(StatType.AttackSpeed);
    public float ExpAttractionRadius => modifierSystem.GetStat(StatType.ExpAttractionRadius);

    public DamageType CurrentDamageType { get; private set; } = DamageType.Physical;

    public event Action OnStatsRecalculated;

    private void Awake()
    {
        modifierSystem = GetComponent<PlayerModifierSystem>();
        modifierSystem.OnStatChanged += (type, value) =>
        {
            OnStatsRecalculated?.Invoke();
        };
    }

    public void SetDamageType(DamageType type)
    {
        CurrentDamageType = type;
    }
}