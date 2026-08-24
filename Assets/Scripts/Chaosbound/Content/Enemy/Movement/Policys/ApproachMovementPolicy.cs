using System;

public sealed class ApproachMovementPolicy :
    IEnemyMovementPolicy
{
    public EnemyMovementIntent Evaluate(
        EnemyMovementPolicyContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(
                nameof(context));
        }

        return EnemyMovementIntent.MoveToTarget(
            context.TargetPosition);
    }
}