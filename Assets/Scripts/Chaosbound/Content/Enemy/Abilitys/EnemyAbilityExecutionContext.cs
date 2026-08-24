using UnityEngine;

public struct EnemyAbilityExecutionContext
{
    public Transform Owner;
    public Vector3 Origin;
    public Vector3 Direction;

    public float Damage;
    public DamageType DamageType;
    public float AttackRange;

    public EnemyAbilityExecutionContext(
        Transform owner,
        Vector3 origin,
        Vector3 direction,
        float damage,
        DamageType damageType,
        float attackRange)
    {
        Owner = owner;
        Origin = origin;
        Direction = direction;
        Damage = damage;
        DamageType = damageType;
        AttackRange = attackRange;
    }
}