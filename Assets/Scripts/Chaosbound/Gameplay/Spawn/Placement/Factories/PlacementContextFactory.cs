using System;
using UnityEngine;
using Chaosbound.Gameplay.Spawn.Placement.Models;

namespace Chaosbound.Gameplay.Spawn.Placement.Factories
{
    /// <summary>
    /// Creates immutable PlacementContext instances.
    /// </summary>
    public sealed class PlacementContextFactory
    {
        /// <summary>
        /// Creates a PlacementContext.
        /// </summary>
        public PlacementContext Create(
            PlacementIntent intent,
            Transform reference)
        {
            if (intent == null)
                throw new ArgumentNullException(nameof(intent));

            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            return new PlacementContext(
                intent,
                reference);
        }
    }
}