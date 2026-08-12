using Chaosbound.Content.Expeditions.Profiles.Combat;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat
{
    [Serializable]
    public sealed class CombatTargetProgressionAuthoring
    {
        [SerializeField]
        private CombatTargetProgressionProfile m_Profile;

        public CombatTargetProgressionProfile Profile =>
            m_Profile;
    }
}