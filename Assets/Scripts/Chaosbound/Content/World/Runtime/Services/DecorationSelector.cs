using System;
using System.Collections.Generic;
using Chaosbound.Content.World.Themes.Decorations;
using UnityEngine;

namespace Chaosbound.Content.World.Runtime.Services
{
    public sealed class DecorationSelector
    {
        private static IReadOnlyList<DecorationPrefabEntry> GetEntries(
    DecorationProfile profile,
    DecorationContext context)
        {
            return context switch
            {
                DecorationContext.Prop => profile.Props,

                DecorationContext.Obstacle => profile.Obstacles,

                DecorationContext.LargeObstacle => profile.LargeObstacles,

                DecorationContext.Light => profile.Lights,

                _ => throw new ArgumentOutOfRangeException(nameof(context))
            };
        }

        public DecorationPrefabEntry Select(
    DecorationProfile profile,
    DecorationContext context)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            IReadOnlyList<DecorationPrefabEntry> entries =
                GetEntries(profile, context);

            int totalWeight = 0;

            foreach (DecorationPrefabEntry entry in entries)
            {
                if (!IsValid(entry))
                    continue;

                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0)
                return null;

            int roll = UnityEngine.Random.Range(0, totalWeight);
            int current = 0;

            foreach (DecorationPrefabEntry entry in entries)
            {
                if (!IsValid(entry))
                    continue;

                current += entry.Weight;

                if (roll < current)
                    return entry;
            }

            return null;
        }

        private static bool IsValid(DecorationPrefabEntry entry)
        {
            return entry != null
                && entry.Prefab != null
                && entry.Weight > 0;
        }
    }
}