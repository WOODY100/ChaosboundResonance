using System;

public static class EnemyMovementPolicyResolver
{
    public static IEnemyMovementPolicy Resolve(
        EnemyMovementPolicyType type)
    {
        switch (type)
        {
            case EnemyMovementPolicyType.Approach:
                return new ApproachMovementPolicy();

            case EnemyMovementPolicyType.MaintainDistance:
                return new MaintainDistanceMovementPolicy();

            default:
                throw new InvalidOperationException(
                    $"Unsupported enemy movement policy type: {type}.");
        }
    }
}