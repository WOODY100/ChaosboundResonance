using Chaosbound.Content.Enemy.Bosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    [DisallowMultipleComponent]
    public sealed class BossAreaAttackExecutor : MonoBehaviour, IBossAttackExecutor
    {
        private readonly HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        public Type EffectDefinitionType => typeof(BossAreaEffectDefinition);

        public bool CanExecute(BossAttackEffectDefinition definition)
        {
            return definition is BossAreaEffectDefinition area &&
                   area.Validate(out _) &&
                   PoolManager.Instance != null;
        }

        public bool TryCreateExecution(
            BossAttackExecutionContext context,
            out IBossAttackExecution execution)
        {
            execution = null;

            if (context == null || context.Owner == null ||
                !(context.AttackDefinition.EffectDefinition is BossAreaEffectDefinition))
                return false;

            BossAreaEffectDefinition definition =
                (BossAreaEffectDefinition)context.AttackDefinition.EffectDefinition;

            if (definition.TelegraphPrefab != null &&
                context.AttackDefinition.TargetCapturePolicy != TargetCapturePolicy.AtAttackStart)
            {
                return false;
            }

            execution = new BossAreaAttackExecution(definition);
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
                    is BossAreaEffectDefinition definition) ||
                !definition.Validate(out _))
                return BossAttackMaterializationResult.Invalid;

            if (!releaseContext.HasCapturedTargetPosition)
                return BossAttackMaterializationResult.Invalid;

            if (definition.TelegraphPrefab != null &&
                executionContext.AttackDefinition.TargetCapturePolicy != TargetCapturePolicy.AtAttackStart)
                return BossAttackMaterializationResult.Invalid;

            Vector3 center = releaseContext.CapturedTargetPosition + definition.PositionOffset;
            GameObject visualObject = PoolManager.Instance.Get(
                definition.EffectPrefab,
                center,
                Quaternion.identity);

            if (visualObject == null)
                return BossAttackMaterializationResult.Failed;

            BossAreaEffectVisual visual =
                visualObject.GetComponent<BossAreaEffectVisual>();

            if (visual == null)
            {
                visualObject.GetComponent<PooledObject>()?.ReturnToPool();
                return BossAttackMaterializationResult.Invalid;
            }

            if (!visual.Initialize(definition.VisualDuration, definition.Radius))
                return BossAttackMaterializationResult.Failed;

            damagedTargets.Clear();

            Collider[] overlaps = Physics.OverlapSphere(
                center,
                definition.Radius,
                Physics.AllLayers,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < overlaps.Length; i++)
            {
                Collider overlap = overlaps[i];
                if (overlap == null)
                    continue;

                Transform hitTransform = overlap.transform;
                Transform target = executionContext.Target;

                bool belongsToTarget =
                    hitTransform == target ||
                    hitTransform.IsChildOf(target) ||
                    target.IsChildOf(hitTransform);

                if (!belongsToTarget)
                    continue;

                IDamageable damageable = overlap.GetComponentInParent<IDamageable>();
                if (damageable == null || damageable.IsDead ||
                    !damagedTargets.Add(damageable))
                    continue;

                DamageData damageData = new DamageData(
                    releaseContext.EffectiveDamage * definition.DamageMultiplier);
                damageData.source = executionContext.Owner.gameObject;
                damageData.analyticsSourceCategory = "BossAreaAttack";
                damageable.TakeDamage(damageData);
            }

            return BossAttackMaterializationResult.Success;
        }
    }
}
