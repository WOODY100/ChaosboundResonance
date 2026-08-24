using UnityEngine;

public sealed class EnemyProjectileAbilityExecutor
    : IEnemyAbilityExecutor
{
    private EnemyRuntimeAbility ability;

    public void Initialize(
        EnemyRuntimeAbility runtimeAbility)
    {
        ResetExecutor();

        if (runtimeAbility == null)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor received a null EnemyRuntimeAbility.");

            return;
        }

        if (!runtimeAbility.IsInitialized)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor received an uninitialized EnemyRuntimeAbility.");

            return;
        }

        ability = runtimeAbility;
    }

    public void Execute(
        EnemyAbilityExecutionContext context)
    {
        if (ability == null)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor cannot execute without an initialized ability.");

            return;
        }

        EnemyAbilityDefinition definition =
            ability.Definition;

        if (definition == null)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor has no EnemyAbilityDefinition.");

            return;
        }

        if (definition.ProjectilePrefab == null)
        {
            Debug.LogError(
                $"Enemy Ability '{definition.ContentId}' has no projectile prefab.");

            return;
        }

        if (definition.ProjectileSpeed <= 0f)
        {
            Debug.LogError(
                $"Enemy Ability '{definition.ContentId}' has an invalid projectile speed.");

            return;
        }

        if (context.Owner == null)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor received a context with no owner.");

            return;
        }

        if (context.Direction.sqrMagnitude < 0.0001f)
        {
            Debug.LogError(
                "EnemyProjectileAbilityExecutor received an invalid direction.");

            return;
        }

        Vector3 direction =
            context.Direction.normalized;

        float maxDistance =
            context.AttackRange
            + definition.ProjectileRangeExtension;

        float lifetime =
            maxDistance
            / definition.ProjectileSpeed;

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        GameObject projectileObject =
            PoolManager.Instance.Get(
                definition.ProjectilePrefab,
                context.Origin,
                rotation);

        if (projectileObject == null)
        {
            Debug.LogError(
                $"Failed to obtain projectile from pool for ability '{definition.ContentId}'.");

            return;
        }

        EnemyProjectile projectile =
            projectileObject.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"Projectile prefab '{definition.ProjectilePrefab.name}' " +
                "does not contain an EnemyProjectile component.");

            PooledBehaviour pooledBehaviour =
                projectileObject.GetComponent<PooledBehaviour>();

            if (pooledBehaviour != null)
                pooledBehaviour.ReturnToPool();

            return;
        }

        projectile.Initialize(
            context.Owner,
            direction,
            definition.ProjectileSpeed,
            lifetime,
            context.Damage,
            context.DamageType);
    }

    public void ResetExecutor()
    {
        ability = null;
    }
}