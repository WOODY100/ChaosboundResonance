using Chaosbound.Content.Expeditions.Enums.Spawn;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Spawn
{
    [Serializable]
    public sealed class SpawnAuthoring
    {
        [SerializeField]
        private SpawnPlacementPolicy m_Placement;
        [SerializeField]
        private List<SpawnConstraintPolicy> m_Constraints = new();

        public SpawnPlacementPolicy Placement
        {
            get { return m_Placement; }
        }
        public IReadOnlyList<SpawnConstraintPolicy> SpawnConstraints
        {
            get { return m_Constraints; }
        }
    }
}