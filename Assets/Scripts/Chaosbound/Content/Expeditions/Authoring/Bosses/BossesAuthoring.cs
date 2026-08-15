using Chaosbound.Content.Enemy.Bosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Bosses
{
    [Serializable]
    public sealed class BossesAuthoring
    {
        [SerializeField]
        private List<BossData> m_Content = new();

        public IReadOnlyList<BossData> Content =>
            m_Content;
    }
}