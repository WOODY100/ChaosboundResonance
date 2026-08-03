using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Debugging;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Placement.Resolvers
{
    /// <summary>
    /// Resolves the placement strategy associated
    /// with a placement policy.
    /// </summary>
    public sealed class PlacementResolver
    {
        private readonly IReadOnlyDictionary<
            SpawnPlacementPolicy,
            ISpawnPlacementStrategy> strategies;

        /// <summary>
        /// Creates a new PlacementResolver.
        /// </summary>
        public PlacementResolver(
            ISpawnPlacementStrategy aroundPlayerStrategy)
        {
            if (aroundPlayerStrategy == null)
                throw new ArgumentNullException(
                    nameof(aroundPlayerStrategy));

            strategies =
                new Dictionary<
                    SpawnPlacementPolicy,
                    ISpawnPlacementStrategy>
                {
                    {
                        SpawnPlacementPolicy.AroundPlayer,
                        aroundPlayerStrategy
                    }
                };
        }

        /// <summary>
        /// Resolves a placement for the supplied context.
        /// </summary>
        public PlacementResolution Resolve(
            PlacementContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            SpawnPlacementPolicy policy =
                context.Intent.PlacementPolicy;

            if (!strategies.TryGetValue(
                policy,
                out ISpawnPlacementStrategy strategy))
            {
                throw new InvalidOperationException(
                    $"No placement strategy registered for '{policy}'.");
            }

            PlacementResolution resolution =
                strategy.Resolve(context);

            SpawnRuntimeDebugger.LogPlacement(
                context,
                resolution);

            return resolution;
        }
    }
}