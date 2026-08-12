using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat
{
    [Serializable]
    public sealed class CombatAuthoring
    {
        [Header("Target Progression")]

        [SerializeField]
        private CombatTargetProgressionAuthoring
            m_TargetProgression = new();

        public CombatTargetProgressionAuthoring TargetProgression =>
            m_TargetProgression;

        [Header("Tactics")]

        [SerializeField]
        private List<CombatTacticAuthoring>
            m_Tactics = new();

        public IReadOnlyList<CombatTacticAuthoring> Tactics =>
            m_Tactics;
    }
}