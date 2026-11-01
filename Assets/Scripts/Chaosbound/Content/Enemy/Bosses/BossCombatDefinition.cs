using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(menuName = "Chaosbound/Bosses/Combat Definition")]
    public sealed class BossCombatDefinition : ScriptableObject
    {
        [Header("Base Stats")]
        [SerializeField] private BossBaseStats m_BaseStats;

        [Header("Movement")]
        [SerializeField] private BossMovementSettings m_MovementSettings;

        [Header("Combat")]
        [SerializeField] private BossCombatSettings m_CombatSettings;

        [Header("Phases")]
        [SerializeField] private List<BossPhaseDefinition> m_Phases = new();

        public BossBaseStats BaseStats => m_BaseStats;
        public BossMovementSettings MovementSettings => m_MovementSettings;
        public BossCombatSettings CombatSettings => m_CombatSettings;
        public IReadOnlyList<BossPhaseDefinition> Phases => m_Phases;

        public bool Validate(out string error)
        {
            if (!m_BaseStats.IsValid)
                return Fail("BaseStats must have MaxHP > 0 and BaseDamage > 0.", out error);
            if (!m_MovementSettings.IsValid)
                return Fail("MovementSettings contains invalid values.", out error);
            if (!m_CombatSettings.IsValid)
                return Fail("WaitReevaluationInterval must be > 0.", out error);
            if (m_Phases == null || m_Phases.Count == 0)
                return Fail("At least one phase is required.", out error);

            HashSet<BossPhaseDefinition> phaseReferences = new HashSet<BossPhaseDefinition>();
            HashSet<string> phaseIds = new HashSet<string>(StringComparer.Ordinal);
            float previousThreshold = float.PositiveInfinity;

            for (int i = 0; i < m_Phases.Count; i++)
            {
                BossPhaseDefinition phase = m_Phases[i];
                if (phase == null || !phaseReferences.Add(phase))
                    return Fail("Phases contains a null or duplicate reference.", out error);
                if (!phase.Validate(out string phaseError))
                    return Fail($"Phase '{phase.name}' is invalid: {phaseError}", out error);
                if (!phaseIds.Add(phase.PhaseId))
                    return Fail($"Duplicate PhaseId '{phase.PhaseId}'.", out error);

                // First phase is always the initial phase. Later thresholds
                // must descend strictly; the first threshold is not used to
                // select the initial phase.
                if (i > 0)
                {
                    if (phase.HealthThreshold >= previousThreshold)
                        return Fail("Phase thresholds must descend strictly after the initial phase.", out error);
                    if (phase.HealthThreshold >= 100f)
                        return Fail("Transition phase thresholds must be below 100%.", out error);
                }
                previousThreshold = phase.HealthThreshold;

                bool hasAttackForPreferredDistance = false;
                foreach (BossAttackDefinition attack in phase.AvailableAttacks)
                {
                    if (!attack.AvailablePhases.Contains(phase.PhaseId))
                        return Fail($"Attack '{attack.name}' does not include phase '{phase.PhaseId}' in AvailablePhases.", out error);

                    if (attack.MinimumRange <= m_MovementSettings.PreferredDistance &&
                        m_MovementSettings.PreferredDistance <= attack.MaximumRange)
                        hasAttackForPreferredDistance = true;
                }

                if (!hasAttackForPreferredDistance)
                    return Fail($"Phase '{phase.PhaseId}' has no attack valid at PreferredDistance.", out error);
            }

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
                Debug.LogWarning($"BossCombatDefinition '{name}': {error}", this);
        }
    }
}
