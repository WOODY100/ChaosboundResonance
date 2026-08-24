using UnityEngine;

public sealed class EnemyMovementPolicyContext
{
    public Vector3 EnemyPosition
    {
        get;
    }

    public Vector3 TargetPosition
    {
        get;
    }

    public float CurrentDistance
    {
        get;
    }

    public float PreferredDistance
    {
        get;
    }

    public float DistanceTolerance
    {
        get;
    }

    public EnemyMovementPolicyContext(
        Vector3 enemyPosition,
        Vector3 targetPosition,
        float preferredDistance,
        float distanceTolerance)
    {
        if (preferredDistance <= 0f)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(preferredDistance));
        }

        if (distanceTolerance < 0f)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(distanceTolerance));
        }

        EnemyPosition =
            enemyPosition;

        TargetPosition =
            targetPosition;

        PreferredDistance =
            preferredDistance;

        DistanceTolerance =
            distanceTolerance;

        Vector3 offset =
            enemyPosition -
            targetPosition;

        offset.y = 0f;

        CurrentDistance =
            offset.magnitude;
    }
}