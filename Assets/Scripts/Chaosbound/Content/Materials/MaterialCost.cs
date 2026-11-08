using System;
using UnityEngine;

namespace Chaosbound.Content.Materials
{
    [Serializable]
    public struct MaterialCost
    {
        [SerializeField]
        private string contentId;

        [SerializeField]
        private int amount;

        public string ContentId => contentId;
        public int Amount => amount;

        public MaterialCost(
            string contentId,
            int amount)
        {
            this.contentId = contentId;
            this.amount = amount;
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(contentId)
            && amount > 0;
    }
}