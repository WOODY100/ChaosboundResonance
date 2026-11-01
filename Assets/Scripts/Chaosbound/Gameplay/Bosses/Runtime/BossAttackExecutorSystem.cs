using System;
using System.Collections.Generic;
using Chaosbound.Content.Enemy.Bosses;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Per-Boss registry and release coordinator.
    /// Create one instance per BossCombatRuntime; do not share it between Bosses.
    /// </summary>
    public sealed class BossAttackExecutorSystem
    {
        private readonly Dictionary<Type, IBossAttackExecutor> executors =
            new Dictionary<Type, IBossAttackExecutor>();

        private readonly Dictionary<string, float> retryRemainingByAttack =
            new Dictionary<string, float>(StringComparer.Ordinal);

        private readonly HashSet<string> invalidAttackIds =
            new HashSet<string>(StringComparer.Ordinal);

        private readonly HashSet<string> diagnosticMessages =
            new HashSet<string>(StringComparer.Ordinal);

        private readonly float failureRetryDelay;

        public BossAttackExecutorSystem(float failureRetryDelay)
        {
            if (failureRetryDelay < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(failureRetryDelay),
                    "FailureRetryDelay must be >= 0.");

            this.failureRetryDelay = failureRetryDelay;
        }

        public float FailureRetryDelay => failureRetryDelay;
        public int RegisteredExecutorCount => executors.Count;

        public bool Register(IBossAttackExecutor executor)
        {
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));

            Type definitionType = executor.EffectDefinitionType;

            if (definitionType == null ||
                !typeof(BossAttackEffectDefinition).IsAssignableFrom(definitionType) ||
                definitionType.IsAbstract)
            {
                Debug.LogError(
                    "Boss attack executor has an invalid EffectDefinitionType.");
                return false;
            }

            if (executors.ContainsKey(definitionType))
            {
                Debug.LogError(
                    $"A Boss attack executor is already registered for '{definitionType.FullName}'.");
                return false;
            }

            executors.Add(definitionType, executor);
            return true;
        }

        public bool TryGetExecutor(
            BossAttackEffectDefinition effectDefinition,
            out IBossAttackExecutor executor)
        {
            executor = null;

            if (effectDefinition == null)
                return false;

            return executors.TryGetValue(effectDefinition.GetType(), out executor);
        }

        public bool IsAttackAvailable(BossAttackDefinition attack)
        {
            if (attack == null || string.IsNullOrWhiteSpace(attack.ContentId))
                return false;

            if (invalidAttackIds.Contains(attack.ContentId))
                return false;

            if (retryRemainingByAttack.TryGetValue(
                attack.ContentId, out float remaining) && remaining > 0f)
            {
                return false;
            }

            if (!TryGetExecutor(attack.EffectDefinition, out _))
            {
                LogDiagnosticOnce(
                    "missing-executor:" + attack.ContentId,
                    $"Boss attack '{attack.ContentId}' has no registered executor for exact effect type " +
                    $"'{attack.EffectDefinition.GetType().FullName}'.",
                    attack);
                return false;
            }

            return true;
        }

        /// <summary>
        /// A failed creation is only reported to the caller. It does not start
        /// FailureRetryDelay; the caller excludes this candidate for the
        /// remainder of its current selection cycle.
        /// </summary>
        public bool TryCreateExecution(
            BossAttackExecutionContext context,
            out IBossAttackExecution execution)
        {
            execution = null;

            if (context == null ||
                context.AttackDefinition == null ||
                context.AttackDefinition.EffectDefinition == null)
            {
                return false;
            }

            if (invalidAttackIds.Contains(context.AttackDefinition.ContentId))
                return false;

            if (!TryGetExecutor(
                context.AttackDefinition.EffectDefinition,
                out IBossAttackExecutor executor))
            {
                return false;
            }

            if (!executor.CanExecute(context.AttackDefinition.EffectDefinition))
            {
                LogDiagnosticOnce(
                    "can-execute:" + context.AttackDefinition.ContentId,
                    $"Boss attack '{context.AttackDefinition.ContentId}' was rejected by " +
                    $"'{executor.GetType().Name}.CanExecute()'.",
                    context.AttackDefinition);
                return false;
            }

            if (!executor.TryCreateExecution(context, out execution) ||
                execution == null)
            {
                LogDiagnosticOnce(
                    "create-execution:" + context.AttackDefinition.ContentId,
                    $"Boss attack '{context.AttackDefinition.ContentId}' executor " +
                    $"'{executor.GetType().Name}' could not create its execution.",
                    context.AttackDefinition);
                execution?.Dispose();
                execution = null;
                return false;
            }

            if (!execution.Initialize(context))
            {
                LogDiagnosticOnce(
                    "initialize-execution:" + context.AttackDefinition.ContentId,
                    $"Boss attack '{context.AttackDefinition.ContentId}' execution.Initialize() failed.",
                    context.AttackDefinition);
                execution.Cancel();
                execution.Dispose();
                execution = null;
                return false;
            }

            return true;
        }

        public BossAttackMaterializationResult TryRelease(
            BossAttackExecutionContext executionContext,
            BossAttackReleaseContext releaseContext,
            int releaseIndex)
        {
            if (executionContext == null ||
                executionContext.AttackDefinition == null ||
                executionContext.AttackDefinition.EffectDefinition == null)
            {
                return BossAttackMaterializationResult.Invalid;
            }

            BossAttackDefinition attack = executionContext.AttackDefinition;

            if (invalidAttackIds.Contains(attack.ContentId))
                return BossAttackMaterializationResult.Invalid;

            if (!TryGetExecutor(attack.EffectDefinition, out IBossAttackExecutor executor))
            {
                DisableInvalidAttack(attack, "No executor is registered for its exact effect type.");
                return BossAttackMaterializationResult.Invalid;
            }

            if (!executor.CanExecute(attack.EffectDefinition))
            {
                DisableInvalidAttack(attack, "The registered executor rejected its effect definition.");
                return BossAttackMaterializationResult.Invalid;
            }

            BossAttackMaterializationResult result =
                executor.TryRelease(executionContext, releaseContext, releaseIndex);

            switch (result)
            {
                case BossAttackMaterializationResult.Success:
                    retryRemainingByAttack.Remove(attack.ContentId);
                    break;

                case BossAttackMaterializationResult.Failed:
                    retryRemainingByAttack[attack.ContentId] = failureRetryDelay;
                    break;

                case BossAttackMaterializationResult.Invalid:
                    DisableInvalidAttack(attack, "Executor reported an invalid release.");
                    break;
            }

            return result;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || retryRemainingByAttack.Count == 0)
                return;

            List<string> attackIds = new List<string>(retryRemainingByAttack.Keys);
            List<string> completedRetries = new List<string>();

            for (int i = 0; i < attackIds.Count; i++)
            {
                string attackId = attackIds[i];
                float remaining = retryRemainingByAttack[attackId] - deltaTime;

                if (remaining <= 0f)
                    completedRetries.Add(attackId);
                else
                    retryRemainingByAttack[attackId] = remaining;
            }

            for (int i = 0; i < completedRetries.Count; i++)
                retryRemainingByAttack.Remove(completedRetries[i]);
        }

        public void ResetEncounter()
        {
            retryRemainingByAttack.Clear();
            invalidAttackIds.Clear();
            diagnosticMessages.Clear();
        }


        private void LogDiagnosticOnce(
            string key,
            string message,
            UnityEngine.Object context)
        {
            if (!diagnosticMessages.Add(key))
                return;

            Debug.LogWarning(message, context);
        }

        private void DisableInvalidAttack(
            BossAttackDefinition attack,
            string reason)
        {
            if (!invalidAttackIds.Add(attack.ContentId))
                return;

            Debug.LogError(
                $"Boss attack '{attack.ContentId}' was disabled: {reason}",
                attack);
        }
    }
}
