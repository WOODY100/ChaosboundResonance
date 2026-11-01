using System;
using Chaosbound.Content.Enemy.Bosses;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
/// <summary>
/// Wraps the shared timed attack clock with the optional area telegraph lifecycle.
/// The telegraph is created at attack-start target position and returned immediately
/// before the release is forwarded to BossCombatRuntime.
/// </summary>
public sealed class BossAreaAttackExecution : IBossAttackExecution
{
    private readonly BossAreaEffectDefinition definition;
    private BossTimedAttackExecution timedExecution;
    private GameObject telegraphObject;
    private BossAreaTelegraphVisual telegraphVisual;
    private bool disposed;

    public BossAttackExecutionState State =>
        timedExecution != null
            ? timedExecution.State
            : BossAttackExecutionState.Uninitialized;

    public event Action<int> ReleaseRequested;
    public event Action<BossAttackExecutionState> StateChanged;

    public BossAreaAttackExecution(BossAreaEffectDefinition definition)
    {
        this.definition = definition;
    }

    public bool Initialize(BossAttackExecutionContext context)
    {
        if (disposed || context == null || definition == null ||
            context.AttackDefinition == null)
            return false;

        timedExecution = new BossTimedAttackExecution();
        timedExecution.ReleaseRequested += HandleReleaseRequested;
        timedExecution.StateChanged += HandleStateChanged;

        if (!timedExecution.Initialize(context))
        {
            ReturnTelegraph();
            return false;
        }

        if (definition.TelegraphPrefab == null)
            return true;

        if (context.AttackDefinition.TargetCapturePolicy != TargetCapturePolicy.AtAttackStart ||
            PoolManager.Instance == null)
        {
            timedExecution.Cancel();
            ReturnTelegraph();
            return false;
        }

        Vector3 telegraphPosition =
            context.TargetPositionAtAttackStart + definition.PositionOffset;

        telegraphObject = PoolManager.Instance.Get(
            definition.TelegraphPrefab,
            telegraphPosition,
            Quaternion.identity);

        if (telegraphObject == null)
        {
            timedExecution.Cancel();
            return false;
        }

        telegraphVisual =
            telegraphObject.GetComponent<BossAreaTelegraphVisual>();

        if (telegraphVisual == null ||
            !telegraphVisual.Initialize(definition.Radius))
        {
            ReturnTelegraph();
            timedExecution.Cancel();
            return false;
        }

        return true;
    }

    public void Tick(float deltaTime)
    {
        if (disposed || timedExecution == null)
            return;

        timedExecution.Tick(deltaTime);
    }

    public void Cancel()
    {
        if (disposed)
            return;

        ReturnTelegraph();
        timedExecution?.Cancel();
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        ReturnTelegraph();

        if (timedExecution != null)
        {
            timedExecution.ReleaseRequested -= HandleReleaseRequested;
            timedExecution.StateChanged -= HandleStateChanged;
            timedExecution.Dispose();
            timedExecution = null;
        }

        ReleaseRequested = null;
        StateChanged = null;
    }

    private void HandleReleaseRequested(int releaseIndex)
    {
        // Hide the warning before the executor materializes the damaging effect.
        ReturnTelegraph();
        ReleaseRequested?.Invoke(releaseIndex);
    }

    private void HandleStateChanged(BossAttackExecutionState state)
    {
        if (state == BossAttackExecutionState.Cancelled ||
            state == BossAttackExecutionState.Failed ||
            state == BossAttackExecutionState.Completed)
        {
            ReturnTelegraph();
        }

        StateChanged?.Invoke(state);
    }

    private void ReturnTelegraph()
    {
        if (telegraphObject == null)
            return;

        PooledObject pooledObject =
            telegraphObject.GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            UnityEngine.Object.Destroy(telegraphObject);

        telegraphObject = null;
        telegraphVisual = null;
    }
}
}
