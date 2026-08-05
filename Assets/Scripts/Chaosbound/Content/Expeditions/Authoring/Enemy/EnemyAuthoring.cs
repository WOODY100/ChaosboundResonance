using Chaosbound.Content.Expeditions.Authoring.Enemy.TacticalIdentity;
using Chaosbound.Content.Expeditions.Enums.Enemy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Enemy
{
    [Serializable]
    public sealed class EnemyAuthoring
    {
        [SerializeField]
        private List<EnemyVariantData> m_Content = new();

        public IReadOnlyList<EnemyVariantData> Content => m_Content;

        [SerializeField]
        private EnemySchedulingPolicy m_SchedulingPolicy =
            EnemySchedulingPolicy.Continuous;

        public EnemySchedulingPolicy SchedulingPolicy =>
            m_SchedulingPolicy;

        [SerializeField]
        private TacticalIdentityAuthoring m_TacticalIdentity =
            new();

        public TacticalIdentityAuthoring TacticalIdentity =>
            m_TacticalIdentity;
    }
}