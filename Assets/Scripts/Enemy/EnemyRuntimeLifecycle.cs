using System;
using UnityEngine;

/// <summary>
/// Synchronizes the enemy lifecycle with the Expedition Runtime.
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyRuntimeContext))]
public sealed class EnemyRuntimeLifecycle : MonoBehaviour
{
    private EnemyHealth health;

    private EnemyRuntimeContext runtimeContext;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        runtimeContext = GetComponent<EnemyRuntimeContext>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath(
        EnemyHealth enemyHealth)
    {
        if (!runtimeContext.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext has not been initialized.");
        }

        runtimeContext
            .ExpeditionRuntime
            .RuntimeComposition
            .Decrement(
                runtimeContext.Variant);

        runtimeContext
            .ExpeditionRuntime
            .ThreatBudget
            .Release(
                runtimeContext
                    .Variant
                    .ThreatCost);
    }
}