using System;
using Chaosbound.Gameplay.Spawn.Reference.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Reference.Providers
{
    /// <summary>
    /// Resolves the player transform used
    /// by the Spawn Runtime.
    /// </summary>
    public sealed class PlayerReferenceProvider :
        ISpawnReferenceProvider
    {
        /// <summary>
        /// Resolves the player reference.
        /// </summary>
        public SpawnReferenceResult Resolve(
            SpawnReferenceContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Transform player =
                context
                    .References
                    .Player;

            if (player == null)
            {
                return SpawnReferenceResult.Failure(
                    "Player reference is not available.");
            }

            return SpawnReferenceResult.Success(
                player);
        }
    }
}