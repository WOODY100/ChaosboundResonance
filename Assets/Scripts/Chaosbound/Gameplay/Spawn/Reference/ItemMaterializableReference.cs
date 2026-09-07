using System;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.References
{
    /// <summary>
    /// Runtime reference to a physical Item that must be
    /// materialized by the Spawn Runtime.
    /// </summary>
    public sealed class ItemMaterializableReference :
        IMaterializableReference,
        ISpawnPrefabReference
    {
        public string ContentId { get; }

        public GameObject SpawnPrefab { get; }

        public ItemMaterializableReference(
            string contentId,
            GameObject spawnPrefab)
        {
            if (string.IsNullOrEmpty(contentId))
                throw new ArgumentException(
                    "ContentId cannot be null or empty.",
                    nameof(contentId));

            if (spawnPrefab == null)
                throw new ArgumentNullException(
                    nameof(spawnPrefab));

            ContentId = contentId;
            SpawnPrefab = spawnPrefab;
        }
    }
}