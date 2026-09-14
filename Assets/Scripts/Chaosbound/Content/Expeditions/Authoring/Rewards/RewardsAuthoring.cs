using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Rewards
{
    [Serializable]
    public sealed class RewardsAuthoring
    {
        [Header("Completion Reward")]

        [SerializeField]
        private CompletionRewardAuthoring m_completion =
            new CompletionRewardAuthoring();

        public CompletionRewardAuthoring Completion =>
            m_completion;
    }
}