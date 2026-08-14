using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Timeline
{
    [Serializable]
    public sealed class ExpeditionCompletionTargetAuthoring
    {
        [SerializeField]
        private float m_TimeSeconds = 20f;

        public float TimeSeconds => m_TimeSeconds;
    }
}