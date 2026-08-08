using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat
{
    [Serializable]
    public sealed class CombatAuthoring
    {
        [SerializeField]
        private List<CombatTacticAuthoring> m_Tactics = new();

        public IReadOnlyList<CombatTacticAuthoring> Tactics =>
            m_Tactics;
    }
}