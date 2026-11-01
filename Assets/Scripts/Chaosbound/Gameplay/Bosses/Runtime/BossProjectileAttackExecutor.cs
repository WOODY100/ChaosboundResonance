using Chaosbound.Content.Enemy.Bosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    [DisallowMultipleComponent]
    public sealed class BossProjectileAttackExecutor : MonoBehaviour, IBossAttackExecutor
    {
        private readonly HashSet<int> diagnosedDefinitions = new HashSet<int>();

        public Type EffectDefinitionType => typeof(BossProjectileEffectDefinition);

        public bool CanExecute(BossAttackEffectDefinition definition)
        {
            if (!(definition is BossProjectileEffectDefinition projectile))
                return false;

            if (!projectile.Validate(out string validationError))
            {
                LogDefinitionDiagnostic(
                    projectile,
                    "Projectile definition validation failed: " + validationError);
                return false;
            }

            if (PoolManager.Instance == null)
            {
                LogDefinitionDiagnostic(
                    projectile,
                    "PoolManager.Instance is null when validating the projectile attack.");
                return false;
            }

            return true;
        }

        private void LogDefinitionDiagnostic(
            BossProjectileEffectDefinition definition,
            string reason)
        {
            if (definition == null || !diagnosedDefinitions.Add(definition.GetInstanceID()))
                return;

            Debug.LogError(
                $"BossProjectileAttackExecutor on '{name}' rejected " +
                $"'{definition.name}': {reason}",
                this);
        }

        public bool TryCreateExecution(
            BossAttackExecutionContext context,
            out IBossAttackExecution execution)
        {
            execution = null;

            if (context == null || context.Owner == null ||
                !(context.AttackDefinition.EffectDefinition is BossProjectileEffectDefinition))
                return false;

            execution = new BossTimedAttackExecution();
            return true;
        }

        public BossAttackMaterializationResult TryRelease(
            BossAttackExecutionContext executionContext,
            BossAttackReleaseContext releaseContext,
            int releaseIndex)
        {
            if (releaseIndex != 0 ||
                executionContext == null ||
                executionContext.Owner == null ||
                executionContext.Target == null ||
                !(executionContext.AttackDefinition.EffectDefinition
                    is BossProjectileEffectDefinition definition) ||
                !definition.Validate(out _))
                return BossAttackMaterializationResult.Invalid;

            Vector3 direction = releaseContext.CapturedDirection;
            if (releaseContext.HasCapturedTargetPosition)
            {
                direction = releaseContext.CapturedTargetPosition - releaseContext.Origin;
                direction.y = 0f;
            }

            if (direction.sqrMagnitude < 0.0001f)
            {
                Debug.LogWarning(
                    $"Boss projectile attack '{executionContext.AttackDefinition.ContentId}' " +
                    "failed release because its horizontal direction is zero.",
                    this);
                return BossAttackMaterializationResult.Failed;
            }

            Vector3 offset = executionContext.Owner.transform.TransformVector(
                definition.LocalSpawnOffset);
            Vector3 spawnPosition = releaseContext.Origin + offset;
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);

            GameObject projectileObject = PoolManager.Instance.Get(
                definition.ProjectilePrefab,
                spawnPosition,
                rotation);

            if (projectileObject == null)
            {
                Debug.LogError(
                    $"PoolManager.Get returned null for projectile prefab " +
                    $"'{definition.ProjectilePrefab.name}'.",
                    this);
                return BossAttackMaterializationResult.Failed;
            }

            BossProjectileBehaviour projectile =
                projectileObject.GetComponent<BossProjectileBehaviour>();

            if (projectile == null)
            {
                projectileObject.GetComponent<PooledObject>()?.ReturnToPool();
                return BossAttackMaterializationResult.Invalid;
            }

            float lifetime = definition.MaxTravelDistance / definition.Speed;
            float damage = releaseContext.EffectiveDamage * definition.DamageMultiplier;

            if (!projectile.Initialize(
                executionContext.Owner.transform,
                executionContext.Target,
                direction,
                definition.Speed,
                lifetime,
                damage))
            {
                Debug.LogError(
                    $"BossProjectileBehaviour.Initialize failed for " +
                    $"'{definition.ProjectilePrefab.name}'.",
                    projectile);
                return BossAttackMaterializationResult.Failed;
            }

            return BossAttackMaterializationResult.Success;
        }
    }
}
