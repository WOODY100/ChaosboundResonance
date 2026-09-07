using System;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.References
{
    /// <summary>
    /// Runtime reference to a physical Resource that must be
    /// materialized by the Spawn Runtime.
    /// </summary>
    public sealed class ResourceMaterializableReference :
        IMaterializableReference,
        ISpawnPrefabReference
    {
        public string ContentId { get; }

        public GameObject SpawnPrefab { get; }

        public int Amount { get; }

        public ResourceMaterializableReference(
            string contentId,
            GameObject spawnPrefab,
            int amount)
        {
            if (string.IsNullOrEmpty(contentId))
                throw new ArgumentException(
                    "ContentId cannot be null or empty.",
                    nameof(contentId));

            if (spawnPrefab == null)
                throw new ArgumentNullException(
                    nameof(spawnPrefab));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Resource amount must be greater than zero.");

            ContentId = contentId;
            SpawnPrefab = spawnPrefab;
            Amount = amount;
        }
    }
}