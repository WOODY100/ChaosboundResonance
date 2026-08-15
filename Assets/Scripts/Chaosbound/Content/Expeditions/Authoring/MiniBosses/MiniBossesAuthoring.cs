using Chaosbound.Content.Enemy.MiniBosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.MiniBosses
{
    [Serializable]
    public sealed class MiniBossesAuthoring
    {
        [SerializeField]
        private List<MiniBossData> m_Content = new();

        public IReadOnlyList<MiniBossData> Content =>
            m_Content;
    }
}