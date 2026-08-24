using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Debugging;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Placement.Validation;
using Chaosbound.Gameplay.Spawn.Placement.ValueObjects;
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

        private const int MaxPlacementAttempts = 8;

        private readonly PlacementValidator
            placementValidator;

        /// <summary>
        /// Creates a new PlacementResolver.
        /// </summary>
        public PlacementResolver(
            ISpawnPlacementStrategy aroundPlayerStrategy,
            ISpawnPlacementStrategy nearReferenceStrategy,
            PlacementValidator placementValidator)
        {
            if (aroundPlayerStrategy == null)
                throw new ArgumentNullException(
                    nameof(aroundPlayerStrategy));

            if (nearReferenceStrategy == null)
                throw new ArgumentNullException(
                    nameof(nearReferenceStrategy));

            this.placementValidator =
                placementValidator
                ?? throw new ArgumentNullException(
                    nameof(placementValidator));

            strategies =
            new Dictionary<
                SpawnPlacementPolicy,
                ISpawnPlacementStrategy>
            {
                {
                    SpawnPlacementPolicy.AroundPlayer,
                    aroundPlayerStrategy
                },
                {
                    SpawnPlacementPolicy.AroundCompletionOrigin,
                    nearReferenceStrategy
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

            PlacementResolution lastResolution = null;

            for (int attempt = 0;
                 attempt < MaxPlacementAttempts;
                 attempt++)
            {
                PlacementResolution candidate =
                    strategy.Resolve(context);

                if (!candidate.IsSuccess)
                {
                    lastResolution = candidate;
                    continue;
                }

                PlacementResolution validation =
                    placementValidator.Validate(
                        context.Intent.Materializable.Reference,
                        candidate.Placement);

                if (validation.IsSuccess)
                {
                    SpawnRuntimeDebugger.LogPlacement(
                        context,
                        validation);

                    return validation;
                }

                lastResolution = validation;
            }

            PlacementResolution failure =
                lastResolution
                ?? PlacementResolution.Failure(
                    FailureReason.NoSpaceAvailable);

            SpawnRuntimeDebugger.LogPlacement(
                context,
                failure);

            return failure;
        }
    }
}