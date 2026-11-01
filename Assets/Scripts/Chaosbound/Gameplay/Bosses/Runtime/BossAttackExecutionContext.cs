using Chaosbound.Content.Enemy.Bosses;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Immutable sequence-level snapshot supplied to an attack execution.
    /// It intentionally contains no timers or mutable execution state.
    /// </summary>
    public sealed class BossAttackExecutionContext
    {
        public BossCombatRuntime Owner { get; }
        public Transform Target { get; }
        public BossAttackDefinition AttackDefinition { get; }
        public string CurrentPhase { get; }
        public float EffectiveDamage { get; }
        public Vector3 TargetPositionAtAttackStart { get; }
        public Vector3 OriginAtAttackStart { get; }

        public BossAttackExecutionContext(
            BossCombatRuntime owner,
            Transform target,
            BossAttackDefinition attackDefinition,
            string currentPhase,
            float effectiveDamage,
            Vector3 targetPositionAtAttackStart,
            Vector3 originAtAttackStart)
        {
            Owner = owner;
            Target = target;
            AttackDefinition = attackDefinition;
            CurrentPhase = currentPhase;
            EffectiveDamage = effectiveDamage;
            TargetPositionAtAttackStart = targetPositionAtAttackStart;
            OriginAtAttackStart = originAtAttackStart;
        }
    }
}
