using Chaosbound.Content.Enemy.Bosses;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Reusable sequence clock for boss attacks. Presentation is intentionally
    /// separate; this class only owns Anticipation, Execution, release timing,
    /// Recovery and terminal state.
    /// </summary>
    public sealed class BossTimedAttackExecution : IBossAttackExecution
    {
        private BossAttackExecutionContext context;
        private BossAttackExecutionState state = BossAttackExecutionState.Uninitialized;
        private float stageElapsed;
        private float executionElapsed;
        private bool releaseSent;
        private bool disposed;

        public BossAttackExecutionState State => state;

        public event Action<int> ReleaseRequested;
        public event Action<BossAttackExecutionState> StateChanged;

        public bool Initialize(BossAttackExecutionContext executionContext)
        {
            if (executionContext == null ||
                executionContext.AttackDefinition == null ||
                !executionContext.AttackDefinition.Validate(out _))
                return false;

            context = executionContext;
            stageElapsed = 0f;
            executionElapsed = 0f;
            releaseSent = false;
            disposed = false;

            state = BossAttackExecutionState.Anticipation;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (disposed || deltaTime < 0f ||
                state == BossAttackExecutionState.Completed ||
                state == BossAttackExecutionState.Cancelled ||
                state == BossAttackExecutionState.Failed ||
                state == BossAttackExecutionState.Uninitialized)
                return;

            float remaining = deltaTime;
            int safety = 0;

            while (remaining > 0f && safety++ < 8)
            {
                if (state == BossAttackExecutionState.Anticipation)
                {
                    float duration = context.AttackDefinition.Anticipation.Duration;
                    float step = Mathf.Min(remaining, Mathf.Max(0f, duration - stageElapsed));
                    stageElapsed += step;
                    remaining -= step;

                    if (stageElapsed >= duration)
                    {
                        stageElapsed = 0f;
                        ChangeState(BossAttackExecutionState.Execution);
                        continue;
                    }

                    break;
                }

                if (state == BossAttackExecutionState.Execution)
                {
                    float duration = context.AttackDefinition.Execution.Duration;
                    float releaseTime = context.AttackDefinition.Execution.ReleaseTime;
                    float toRelease = releaseSent ? float.PositiveInfinity :
                        Mathf.Max(0f, releaseTime - executionElapsed);
                    float toEnd = Mathf.Max(0f, duration - executionElapsed);
                    float step = Mathf.Min(remaining, Mathf.Min(toRelease, toEnd));

                    executionElapsed += step;
                    stageElapsed = executionElapsed;
                    remaining -= step;

                    if (!releaseSent && executionElapsed >= releaseTime)
                    {
                        releaseSent = true;
                        ReleaseRequested?.Invoke(0);
                        continue;
                    }

                    if (executionElapsed >= duration)
                    {
                        stageElapsed = 0f;
                        ChangeState(BossAttackExecutionState.Recovery);
                        continue;
                    }

                    break;
                }

                if (state == BossAttackExecutionState.Recovery)
                {
                    float duration = context.AttackDefinition.Recovery.Duration;
                    float step = Mathf.Min(remaining, Mathf.Max(0f, duration - stageElapsed));
                    stageElapsed += step;
                    remaining -= step;

                    if (stageElapsed >= duration)
                    {
                        ChangeState(BossAttackExecutionState.Completed);
                        break;
                    }

                    break;
                }

                break;
            }

            // Zero-duration stages must still advance when Tick receives zero time.
            if (deltaTime == 0f)
                AdvanceZeroDurationStages();
        }

        public void Cancel()
        {
            if (disposed || state == BossAttackExecutionState.Completed ||
                state == BossAttackExecutionState.Cancelled)
                return;

            ChangeState(BossAttackExecutionState.Cancelled);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            ReleaseRequested = null;
            StateChanged = null;
            context = null;
        }

        private void AdvanceZeroDurationStages()
        {
            int safety = 0;
            while (safety++ < 4)
            {
                if (state == BossAttackExecutionState.Anticipation &&
                    context.AttackDefinition.Anticipation.Duration <= 0f)
                {
                    ChangeState(BossAttackExecutionState.Execution);
                    continue;
                }

                if (state == BossAttackExecutionState.Execution &&
                    !releaseSent &&
                    context.AttackDefinition.Execution.ReleaseTime <= 0f)
                {
                    releaseSent = true;
                    ReleaseRequested?.Invoke(0);
                    continue;
                }

                if (state == BossAttackExecutionState.Recovery &&
                    context.AttackDefinition.Recovery.Duration <= 0f)
                {
                    ChangeState(BossAttackExecutionState.Completed);
                    continue;
                }

                break;
            }
        }

        private void ChangeState(BossAttackExecutionState next)
        {
            state = next;
            StateChanged?.Invoke(state);
        }
    }
}
