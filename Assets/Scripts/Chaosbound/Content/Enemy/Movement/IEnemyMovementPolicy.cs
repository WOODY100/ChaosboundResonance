public interface IEnemyMovementPolicy
{
    EnemyMovementIntent Evaluate(
        EnemyMovementPolicyContext context);
}