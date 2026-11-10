using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Integration;
using UnityEngine;

/// <summary>
/// Stores the runtime context associated with a materialized enemy.
/// Bridges the Spawn Runtime and the Enemy Runtime.
/// </summary>
public sealed class EnemyRuntimeContext : MonoBehaviour
{
    /// <summary>
    /// Gets the enemy variant represented by this instance.
    /// </summary>
    public EnemyVariantData Variant
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the expedition runtime that owns this enemy.
    /// </summary>
    public ExpeditionRuntimeState ExpeditionRuntime
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets whether this runtime context has been initialized.
    /// </summary>
    public bool IsInitialized
    {
        get;
        private set;
    }

    /// <summary>
    /// Initializes the runtime context.
    /// </summary>
    private ExpeditionEnemyModifierIntegration modifierIntegration;

    /// <summary>
    /// Owns the modifier subscription for this pooled enemy instance.
    /// </summary>
    public void BindModifierIntegration(
        ExpeditionEnemyModifierIntegration integration)
    {
        if (integration == null)
            throw new ArgumentNullException(nameof(integration));

        modifierIntegration?.Dispose();
        modifierIntegration = integration;
    }

    public void Initialize(
        EnemyVariantData variant,
        ExpeditionRuntimeState expeditionRuntime)
    {
        Variant =
            variant
            ?? throw new ArgumentNullException(
                nameof(variant));

        ExpeditionRuntime =
            expeditionRuntime
            ?? throw new ArgumentNullException(
                nameof(expeditionRuntime));

        IsInitialized = true;
    }

    private void OnDisable()
    {
        modifierIntegration?.Dispose();
        modifierIntegration = null;

        Variant = null;
        ExpeditionRuntime = null;
        IsInitialized = false;
    }
}