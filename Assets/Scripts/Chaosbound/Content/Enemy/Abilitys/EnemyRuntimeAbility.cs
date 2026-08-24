using UnityEngine;

public sealed class EnemyRuntimeAbility
{
    private EnemyAbilityDefinition definition;
    private Transform owner;

    public EnemyAbilityDefinition Definition =>
        definition;

    public Transform Owner =>
        owner;

    public bool IsInitialized { get; private set; }

    public void Initialize(
        EnemyAbilityDefinition abilityDefinition,
        Transform abilityOwner)
    {
        Reset();

        if (abilityDefinition == null)
        {
            Debug.LogError(
                "EnemyRuntimeAbility.Initialize received a null EnemyAbilityDefinition.");

            return;
        }

        if (abilityOwner == null)
        {
            Debug.LogError(
                "EnemyRuntimeAbility.Initialize received a null owner.");

            return;
        }

        definition = abilityDefinition;
        owner = abilityOwner;

        IsInitialized = true;
    }

    public void Reset()
    {
        definition = null;
        owner = null;

        IsInitialized = false;
    }
}