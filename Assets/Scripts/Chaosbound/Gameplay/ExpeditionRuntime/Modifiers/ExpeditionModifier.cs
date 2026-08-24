using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    public sealed class ExpeditionModifier
    {
        private readonly IReadOnlyList<ExpeditionModifierEffect> effects;

        public ExpeditionModifierLifetime Lifetime { get; }

        public TimeSpan Duration { get; }

        public TimeSpan CreatedAt { get; }

        public IReadOnlyList<ExpeditionModifierEffect> Effects =>
            effects;

        public bool IsTimed =>
            Lifetime == ExpeditionModifierLifetime.Timed;

        public ExpeditionModifier(
            IEnumerable<ExpeditionModifierEffect> effects,
            ExpeditionModifierLifetime lifetime,
            TimeSpan createdAt,
            TimeSpan duration)
        {
            if (effects == null)
                throw new ArgumentNullException(nameof(effects));

            List<ExpeditionModifierEffect> list =
                effects.ToList();

            if (list.Count == 0)
            {
                throw new ArgumentException(
                    "Modifier must contain at least one effect.",
                    nameof(effects));
            }

            if (lifetime == ExpeditionModifierLifetime.Timed &&
                duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(duration),
                    "Timed modifiers require a positive duration.");
            }

            if (lifetime == ExpeditionModifierLifetime.Expedition)
            {
                duration = TimeSpan.Zero;
            }

            this.effects =
                new ReadOnlyCollection<ExpeditionModifierEffect>(
                    list);

            Lifetime = lifetime;
            CreatedAt = createdAt;
            Duration = duration;
        }

        public bool IsExpired(TimeSpan currentTime)
        {
            if (!IsTimed)
                return false;

            return currentTime >= CreatedAt + Duration;
        }
    }
}