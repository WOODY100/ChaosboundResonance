using System;

public sealed class MaintainDistanceMovementPolicy :
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

        float minimumDistance =
            context.PreferredDistance -
            context.DistanceTolerance;

        float maximumDistance =
            context.PreferredDistance +
            context.DistanceTolerance;

        if (context.CurrentDistance >
            maximumDistance)
        {
            return EnemyMovementIntent.MoveToTarget(
                context.TargetPosition);
        }

        if (context.CurrentDistance <
            minimumDistance)
        {
            return EnemyMovementIntent.Retreat(
                context.TargetPosition,
                context.PreferredDistance);
        }

        return EnemyMovementIntent.None();
    }
}