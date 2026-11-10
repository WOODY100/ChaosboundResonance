using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Immutable snapshot of one runtime modifier application.
    /// The director replaces snapshots when an application is refreshed or stacked.
    /// </summary>
    public sealed class ExpeditionModifier
    {
        private readonly IReadOnlyList<ExpeditionModifierEffect> effects;

        public string ApplicationId { get; }
        public string DefinitionId { get; }
        public string SourceId { get; }
        public ExpeditionModifierLifetime Lifetime { get; }
        public ExpeditionModifierStackingPolicy StackingPolicy { get; }
        public int StackCount { get; }
        public int MaxStacks { get; }
        public TimeSpan BaseDuration { get; }
        public TimeSpan RemainingDuration { get; }
        public TimeSpan TotalGrantedDuration { get; }
        public TimeSpan CreatedAt { get; }
        public TimeSpan LastUpdatedAt { get; }
        public IReadOnlyList<ExpeditionModifierEffect> Effects => effects;
        public bool IsTimed => Lifetime == ExpeditionModifierLifetime.Timed;

        // Legacy compatibility property. New code should use RemainingDuration/BaseDuration.
        public TimeSpan Duration => BaseDuration;

        public ExpeditionModifier(
            IEnumerable<ExpeditionModifierEffect> effects,
            ExpeditionModifierLifetime lifetime,
            TimeSpan createdAt,
            TimeSpan duration)
            : this(
                Guid.NewGuid().ToString("N"),
                "legacy",
                "legacy",
                effects,
                lifetime,
                ExpeditionModifierStackingPolicy.Independent,
                1,
                1,
                duration,
                lifetime == ExpeditionModifierLifetime.Timed ? duration : TimeSpan.Zero,
                lifetime == ExpeditionModifierLifetime.Timed ? duration : TimeSpan.Zero,
                createdAt,
                createdAt)
        {
        }

        public ExpeditionModifier(
            string applicationId,
            string definitionId,
            string sourceId,
            IEnumerable<ExpeditionModifierEffect> effects,
            ExpeditionModifierLifetime lifetime,
            ExpeditionModifierStackingPolicy stackingPolicy,
            int stackCount,
            int maxStacks,
            TimeSpan baseDuration,
            TimeSpan remainingDuration,
            TimeSpan totalGrantedDuration,
            TimeSpan createdAt,
            TimeSpan lastUpdatedAt)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                throw new ArgumentException("ApplicationId cannot be empty.", nameof(applicationId));
            if (string.IsNullOrWhiteSpace(definitionId))
                throw new ArgumentException("DefinitionId cannot be empty.", nameof(definitionId));
            if (effects == null)
                throw new ArgumentNullException(nameof(effects));

            List<ExpeditionModifierEffect> list = effects.ToList();
            if (list.Count == 0)
                throw new ArgumentException("Modifier must contain at least one effect.", nameof(effects));
            if (lifetime == ExpeditionModifierLifetime.Timed && baseDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(baseDuration), "Timed modifiers require a positive base duration.");
            if (stackCount < 1)
                throw new ArgumentOutOfRangeException(nameof(stackCount));
            if (maxStacks < 1)
                throw new ArgumentOutOfRangeException(nameof(maxStacks));
            if (stackCount > maxStacks)
                throw new ArgumentOutOfRangeException(nameof(stackCount));
            if (remainingDuration < TimeSpan.Zero || totalGrantedDuration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(remainingDuration));

            this.effects = new ReadOnlyCollection<ExpeditionModifierEffect>(list);
            ApplicationId = applicationId;
            DefinitionId = definitionId;
            SourceId = sourceId ?? string.Empty;
            Lifetime = lifetime;
            StackingPolicy = stackingPolicy;
            StackCount = stackCount;
            MaxStacks = maxStacks;
            BaseDuration = lifetime == ExpeditionModifierLifetime.Timed ? baseDuration : TimeSpan.Zero;
            RemainingDuration = lifetime == ExpeditionModifierLifetime.Timed ? remainingDuration : TimeSpan.Zero;
            TotalGrantedDuration = lifetime == ExpeditionModifierLifetime.Timed ? totalGrantedDuration : TimeSpan.Zero;
            CreatedAt = createdAt;
            LastUpdatedAt = lastUpdatedAt;
        }

        public bool IsExpired(TimeSpan currentTime)
        {
            if (!IsTimed)
                return false;

            TimeSpan elapsed = currentTime - LastUpdatedAt;
            if (elapsed < TimeSpan.Zero)
                elapsed = TimeSpan.Zero;

            return RemainingDuration - elapsed <= TimeSpan.Zero;
        }

        public ExpeditionModifier WithTiming(
            TimeSpan remainingDuration,
            TimeSpan totalGrantedDuration,
            TimeSpan lastUpdatedAt)
        {
            return Copy(effects, StackCount, remainingDuration, totalGrantedDuration, lastUpdatedAt);
        }

        public ExpeditionModifier WithStacksAndEffects(
            int stackCount,
            IEnumerable<ExpeditionModifierEffect> updatedEffects,
            TimeSpan remainingDuration,
            TimeSpan totalGrantedDuration,
            TimeSpan lastUpdatedAt)
        {
            return Copy(updatedEffects, stackCount, remainingDuration, totalGrantedDuration, lastUpdatedAt);
        }

        private ExpeditionModifier Copy(
            IEnumerable<ExpeditionModifierEffect> updatedEffects,
            int stackCount,
            TimeSpan remainingDuration,
            TimeSpan totalGrantedDuration,
            TimeSpan lastUpdatedAt)
        {
            return new ExpeditionModifier(
                ApplicationId, DefinitionId, SourceId, updatedEffects, Lifetime,
                StackingPolicy, stackCount, MaxStacks, BaseDuration,
                remainingDuration, totalGrantedDuration, CreatedAt, lastUpdatedAt);
        }
    }
}
