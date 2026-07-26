using Chaosbound.Shared.Authoring;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Population
{
    [Serializable]
    public sealed class PopulationAuthoring
    {
        [SerializeField]
        private List<ContentReferenceAuthoring> m_Content = new();

        public IReadOnlyList<ContentReferenceAuthoring> Content => m_Content;
    }
}