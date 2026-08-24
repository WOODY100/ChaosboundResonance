using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeBehavior))]
[RequireComponent(typeof(EnemyRuntimeNavigation))]
public sealed class EnemyRuntimeBehaviorScheduler :
    MonoBehaviour
{
    [SerializeField]
    private float evaluationInterval = 0.1f;

    private EnemyRuntimeBehavior behavior;
    private EnemyRuntimeNavigation navigation;

    private float elapsedTime;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public float EvaluationInterval
    {
        get
        {
            return evaluationInterval;
        }
    }

    private void Awake()
    {
        behavior =
            GetComponent<EnemyRuntimeBehavior>();

        navigation =
            GetComponent<EnemyRuntimeNavigation>();
    }

    public void Initialize()
    {
        if (behavior == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeBehavior is not available.");
        }

        if (!behavior.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeBehavior has not been initialized.");
        }

        if (navigation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation is not available.");
        }

        if (!navigation.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation has not been initialized.");
        }

        if (evaluationInterval <= 0f)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeBehaviorScheduler evaluation interval " +
                "must be greater than zero.");
        }

        elapsedTime = 0f;

        IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized)
            return;

        elapsedTime +=
            Time.deltaTime;

        if (elapsedTime <
            evaluationInterval)
        {
            return;
        }

        elapsedTime = 0f;

        behavior.Tick();

        EnemyMovementIntent intent =
            behavior.ConsumeIntent();

        navigation.ExecuteIntent(
            intent);
    }

    /// <summary>
    /// Stops behavior evaluation without resetting
    /// the complete runtime state.
    /// </summary>
    public void Shutdown()
    {
        elapsedTime = 0f;
        IsInitialized = false;
    }

    public void Reset()
    {
        elapsedTime = 0f;
        IsInitialized = false;
    }

    private void OnDisable()
    {
        Reset();
    }
}