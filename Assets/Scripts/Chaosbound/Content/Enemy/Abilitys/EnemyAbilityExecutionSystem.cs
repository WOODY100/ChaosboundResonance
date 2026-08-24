using UnityEngine;

public sealed class EnemyAbilityExecutionSystem
{
    private readonly IEnemyAbilityExecutor projectileExecutor;

    private IEnemyAbilityExecutor activeExecutor;

    public EnemyAbilityExecutionSystem()
    {
        projectileExecutor =
            new EnemyProjectileAbilityExecutor();
    }

    public void Execute(
        EnemyRuntimeAbility ability,
        EnemyAbilityExecutionContext context)
    {
        if (ability == null)
        {
            Debug.LogError(
                "EnemyAbilityExecutionSystem received a null EnemyRuntimeAbility.");

            return;
        }

        if (!ability.IsInitialized)
        {
            Debug.LogError(
                "EnemyAbilityExecutionSystem received an uninitialized EnemyRuntimeAbility.");

            return;
        }

        IEnemyAbilityExecutor executor =
            ResolveExecutor(
                ability.Definition.ExecutionType);

        if (executor == null)
            return;

        activeExecutor = executor;

        executor.Initialize(ability);
        executor.Execute(context);
    }

    public void Reset()
    {
        if (activeExecutor != null)
        {
            activeExecutor.ResetExecutor();
            activeExecutor = null;
        }

        projectileExecutor.ResetExecutor();
    }

    private IEnemyAbilityExecutor ResolveExecutor(
        EnemyAbilityExecutionType executionType)
    {
        switch (executionType)
        {
            case EnemyAbilityExecutionType.Projectile:
                return projectileExecutor;

            default:
                Debug.LogError(
                    $"No executor registered for EnemyAbilityExecutionType '{executionType}'.");

                return null;
        }
    }
}