using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
public sealed class EnemyRuntimeTargeting : MonoBehaviour
{
    private EnemyRuntimeContext runtimeContext;

    private ITargetProvider targetProvider;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public Transform CurrentTarget
    {
        get;
        private set;
    }

    public bool HasTarget =>
        IsInitialized &&
        CurrentTarget != null;

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();
    }

    public void Initialize(
        ITargetProvider provider)
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

        targetProvider =
            provider
            ?? throw new ArgumentNullException(
                nameof(provider));

        CurrentTarget =
            targetProvider.GetTarget();

        if (CurrentTarget == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting could not resolve a target.");
        }

        IsInitialized = true;
    }

    public void RefreshTarget()
    {
        ValidateInitialized();

        CurrentTarget =
            targetProvider.GetTarget();
    }

    public void Reset()
    {
        CurrentTarget = null;
        targetProvider = null;
        IsInitialized = false;
    }

    private void ValidateInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting has not been initialized.");
        }

        if (targetProvider == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting has no target provider.");
        }
    }

    private void OnDisable()
    {
        Reset();
    }
}