using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Immutable snapshot captured at one release.
    /// Independent effects must not retain the Boss or sequence context.
    /// </summary>
    public readonly struct BossAttackReleaseContext
    {
        public Vector3 Origin { get; }
        public Vector3 CapturedTargetPosition { get; }
        public bool HasCapturedTargetPosition { get; }
        public Vector3 CapturedDirection { get; }
        public float EffectiveDamage { get; }
        public BossEffectOwnership EffectOwnership { get; }

        public BossAttackReleaseContext(
            Vector3 origin,
            Vector3 capturedTargetPosition,
            bool hasCapturedTargetPosition,
            Vector3 capturedDirection,
            float effectiveDamage,
            BossEffectOwnership effectOwnership)
        {
            Origin = origin;
            CapturedTargetPosition = capturedTargetPosition;
            HasCapturedTargetPosition = hasCapturedTargetPosition;
            CapturedDirection = capturedDirection;
            EffectiveDamage = effectiveDamage;
            EffectOwnership = effectOwnership;
        }
    }
}
