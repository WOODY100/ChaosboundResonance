using System;
using System.Collections.Generic;
using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Gameplay.Spawn.Reference.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Models;

namespace Chaosbound.Gameplay.Spawn.Reference.Resolvers
{
    /// <summary>
    /// Resolves the provider responsible for obtaining
    /// the runtime reference required by the Spawn Runtime.
    /// </summary>
    public sealed class SpawnReferenceResolver
    {
        private readonly IReadOnlyDictionary<
            SpawnPlacementPolicy,
            ISpawnReferenceProvider> providers;

        public SpawnReferenceResolver(
            ISpawnReferenceProvider playerProvider)
        {
            if (playerProvider == null)
                throw new ArgumentNullException(
                    nameof(playerProvider));

            providers =
                new Dictionary<
                    SpawnPlacementPolicy,
                    ISpawnReferenceProvider>
                {
                    {
                        SpawnPlacementPolicy.AroundPlayer,
                        playerProvider
                    }
                };
        }

        /// <summary>
        /// Resolves the runtime reference.
        /// </summary>
        public SpawnReferenceResult Resolve(
            SpawnReferenceContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            SpawnPlacementPolicy policy =
                context.SpawnConfig.Placement;

            if (!providers.TryGetValue(
                policy,
                out ISpawnReferenceProvider provider))
            {
                return SpawnReferenceResult.Failure(
                    $"No Spawn Reference Provider registered for '{policy}'.");
            }

            return provider.Resolve(context);
        }
    }
}