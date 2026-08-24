using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
public sealed class EnemyRuntimeStats : MonoBehaviour
{
    private EnemyRuntimeContext runtimeContext;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public float BaseHealth
    {
        get;
        private set;
    }

    public float BaseDamage
    {
        get;
        private set;
    }

    public float BaseMoveSpeed
    {
        get;
        private set;
    }

    public float MaxHealth
    {
        get;
        private set;
    }

    public float Damage
    {
        get;
        private set;
    }

    public float MoveSpeed
    {
        get;
        private set;
    }

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();
    }

    /// <summary>
    /// Initializes runtime stats from the current
    /// enemy runtime context.
    /// </summary>
    public void Initialize()
    {
        if (runtimeContext == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext is not available.");
        }

        if (!runtimeContext.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext has not been initialized.");
        }

        EnemyVariantData variant =
            runtimeContext.Variant;

        if (variant == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext does not contain an EnemyVariantData.");
        }

        BaseHealth =
            variant.BaseHealth;

        BaseDamage =
            variant.BaseDamage;

        BaseMoveSpeed =
            variant.MoveSpeed;

        Refresh();

        IsInitialized = true;
    }

    /// <summary>
    /// Recalculates effective stats from the immutable
    /// base values and the current expedition modifiers.
    /// </summary>
    public void Refresh()
    {
        if (runtimeContext == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext is not available.");
        }

        if (!runtimeContext.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext has not been initialized.");
        }

        if (runtimeContext.ExpeditionRuntime == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext does not contain an ExpeditionRuntime.");
        }

        MaxHealth =
            CalculateEffectiveStat(
                BaseHealth,
                "MaxHealth");

        Damage =
            CalculateEffectiveStat(
                BaseDamage,
                "Damage");

        MoveSpeed =
            CalculateEffectiveStat(
                BaseMoveSpeed,
                "MoveSpeed");
    }

    /// <summary>
    /// Resets the runtime stats so the pooled instance
    /// does not retain state from its previous materialization.
    /// </summary>
    public void Reset()
    {
        BaseHealth = 0f;
        BaseDamage = 0f;
        BaseMoveSpeed = 0f;

        MaxHealth = 0f;
        Damage = 0f;
        MoveSpeed = 0f;

        IsInitialized = false;
    }

    private float CalculateEffectiveStat(
        float baseValue,
        string statId)
    {
        float totalPercent =
            runtimeContext
                .ExpeditionRuntime
                .Modifiers
                .GetTotalPercent(
                    ExpeditionModifierTarget.Enemy,
                    statId,
                    runtimeContext
                        .ExpeditionRuntime
                        .ElapsedTime);

        return baseValue * (1f + totalPercent);
    }

    private void OnDisable()
    {
        Reset();
    }
}