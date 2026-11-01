using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(menuName = "Chaosbound/Bosses/Phase Definition")]
    public sealed class BossPhaseDefinition : ScriptableObject
    {
        [SerializeField] private string m_PhaseId;
        [SerializeField] private string m_DisplayName;
        [SerializeField, Range(0f, 100f)] private float m_HealthThreshold;
        [SerializeField] private List<BossAttackDefinition> m_AvailableAttacks = new();

        public string PhaseId => m_PhaseId;
        public string DisplayName => m_DisplayName;
        public float HealthThreshold => m_HealthThreshold;
        public IReadOnlyList<BossAttackDefinition> AvailableAttacks => m_AvailableAttacks;

        public bool Validate(out string error)
        {
            if (string.IsNullOrWhiteSpace(m_PhaseId))
                return Fail("PhaseId is required.", out error);
            if (string.IsNullOrWhiteSpace(m_DisplayName))
                return Fail("DisplayName is required.", out error);
            if (m_HealthThreshold < 0f || m_HealthThreshold > 100f)
                return Fail("HealthThreshold must be between 0 and 100.", out error);
            if (m_AvailableAttacks == null || m_AvailableAttacks.Count == 0)
                return Fail("At least one attack is required.", out error);

            HashSet<BossAttackDefinition> attacks = new HashSet<BossAttackDefinition>();
            foreach (BossAttackDefinition attack in m_AvailableAttacks)
            {
                if (attack == null || !attacks.Add(attack))
                    return Fail("AvailableAttacks contains a null or duplicate reference.", out error);
                if (!attack.Validate(out string attackError))
                    return Fail($"Attack '{attack.name}' is invalid: {attackError}", out error);
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
                Debug.LogWarning($"BossPhaseDefinition '{name}': {error}", this);
        }
    }
}
