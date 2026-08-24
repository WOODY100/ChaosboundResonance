public interface IEnemyAbilityExecutor
{
    void Initialize(
        EnemyRuntimeAbility ability);

    void Execute(
        EnemyAbilityExecutionContext context);

    void ResetExecutor();
}