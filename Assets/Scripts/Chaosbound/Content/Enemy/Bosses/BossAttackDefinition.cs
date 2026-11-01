using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    public enum TargetCapturePolicy
    {
        AtAttackStart,
        AtExecutionStart,
        AtRelease,
        FixedOrigin
    }

    public enum FacingPolicy
    {
        FaceTargetDuringAnticipation,
        FaceTargetAtAttackStart,
        FixedDirection,
        NoRotation
    }

    [Serializable]
    public struct BossAttackStageDefinition
    {
        [SerializeField, Min(0f)] private float m_Duration;
        [SerializeField] private string m_PresentationVariant;

        public float Duration => m_Duration;
        public string PresentationVariant => m_PresentationVariant;
    }

    [Serializable]
    public struct BossAttackExecutionDefinition
    {
        [SerializeField, Min(0.01f)] private float m_Duration;
        [SerializeField, Min(0f)] private float m_ReleaseTime;

        public float Duration => m_Duration;
        public float ReleaseTime => m_ReleaseTime;
    }

    [CreateAssetMenu(menuName = "Chaosbound/Bosses/Attack Definition")]
    public sealed class BossAttackDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string m_ContentId;
        [SerializeField] private string m_DisplayName;

        [Header("Availability")]
        [SerializeField] private List<string> m_AvailablePhases = new();

        [Header("Selection")]
        [SerializeField, Min(0f)] private float m_Cooldown;
        [SerializeField, Min(0.01f)] private float m_SelectionWeight = 1f;

        [Header("Range")]
        [SerializeField, Min(0f)] private float m_MinimumRange;
        [SerializeField, Min(0f)] private float m_MaximumRange = 10f;

        [Header("Sequence")]
        [SerializeField] private BossAttackStageDefinition m_Anticipation;
        [SerializeField] private BossAttackExecutionDefinition m_Execution;
        [SerializeField] private BossAttackStageDefinition m_Recovery;

        [Header("Targeting")]
        [SerializeField] private TargetCapturePolicy m_TargetCapturePolicy;
        [SerializeField] private FacingPolicy m_FacingPolicy;

        [Header("Effect")]
        [SerializeField] private BossAttackEffectDefinition m_EffectDefinition;

        public string ContentId => m_ContentId;
        public string DisplayName => m_DisplayName;
        public IReadOnlyList<string> AvailablePhases => m_AvailablePhases;
        public float Cooldown => m_Cooldown;
        public float SelectionWeight => m_SelectionWeight;
        public float MinimumRange => m_MinimumRange;
        public float MaximumRange => m_MaximumRange;
        public BossAttackStageDefinition Anticipation => m_Anticipation;
        public BossAttackExecutionDefinition Execution => m_Execution;
        public BossAttackStageDefinition Recovery => m_Recovery;
        public TargetCapturePolicy TargetCapturePolicy => m_TargetCapturePolicy;
        public FacingPolicy FacingPolicy => m_FacingPolicy;
        public BossAttackEffectDefinition EffectDefinition => m_EffectDefinition;

        public bool Validate(out string error)
        {
            if (string.IsNullOrWhiteSpace(m_ContentId))
                return Fail("ContentId is required.", out error);
            if (string.IsNullOrWhiteSpace(m_DisplayName))
                return Fail("DisplayName is required.", out error);
            if (m_AvailablePhases == null || m_AvailablePhases.Count == 0)
                return Fail("At least one AvailablePhase is required.", out error);

            HashSet<string> phases = new HashSet<string>(StringComparer.Ordinal);
            foreach (string phase in m_AvailablePhases)
            {
                if (string.IsNullOrWhiteSpace(phase) || !phases.Add(phase))
                    return Fail("AvailablePhases contains an empty or duplicate PhaseId.", out error);
            }

            if (m_Cooldown < 0f || m_SelectionWeight <= 0f)
                return Fail("Cooldown must be >= 0 and SelectionWeight must be > 0.", out error);
            if (m_MinimumRange < 0f || m_MaximumRange < m_MinimumRange)
                return Fail("Attack range is invalid.", out error);
            if (m_Anticipation.Duration < 0f || m_Recovery.Duration < 0f)
                return Fail("Anticipation and Recovery durations must be >= 0.", out error);
            if (m_Execution.Duration <= 0f || m_Execution.ReleaseTime < 0f || m_Execution.ReleaseTime > m_Execution.Duration)
                return Fail("Execution duration or ReleaseTime is invalid.", out error);
            if (m_EffectDefinition == null)
                return Fail("EffectDefinition is required.", out error);

            error = null;
            return true;
        }

        private static bool Fail(string message, out string error)
        {
            error = message;
            return false;
        }

        private void OnValidate()
        {
            if (!Validate(out string error))
                Debug.LogWarning($"BossAttackDefinition '{name}': {error}", this);
        }
    }
}
