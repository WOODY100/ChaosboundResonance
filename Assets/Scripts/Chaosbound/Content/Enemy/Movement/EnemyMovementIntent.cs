using UnityEngine;

public sealed class EnemyMovementIntent
{
    public EnemyMovementIntentType Type
    {
        get;
    }

    public Vector3 Position
    {
        get;
    }

    public float Distance
    {
        get;
    }

    public EnemyMovementIntent(
        EnemyMovementIntentType type,
        Vector3 position,
        float distance)
    {
        Type = type;
        Position = position;
        Distance = distance;
    }

    public static EnemyMovementIntent None()
    {
        return new EnemyMovementIntent(
            EnemyMovementIntentType.None,
            Vector3.zero,
            0f);
    }

    public static EnemyMovementIntent MoveToTarget(
        Vector3 targetPosition)
    {
        return new EnemyMovementIntent(
            EnemyMovementIntentType.MoveToTarget,
            targetPosition,
            0f);
    }

    public static EnemyMovementIntent MoveToPosition(
        Vector3 position)
    {
        return new EnemyMovementIntent(
            EnemyMovementIntentType.MoveToPosition,
            position,
            0f);
    }

    public static EnemyMovementIntent MaintainDistance(
        Vector3 targetPosition,
        float preferredDistance)
    {
        return new EnemyMovementIntent(
            EnemyMovementIntentType.MaintainDistance,
            targetPosition,
            preferredDistance);
    }

    public static EnemyMovementIntent Retreat(
        Vector3 targetPosition,
        float retreatDistance)
    {
        return new EnemyMovementIntent(
            EnemyMovementIntentType.Retreat,
            targetPosition,
            retreatDistance);
    }
}