using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Coordinates validation, application policies, expiration and cleanup.
    /// It does not calculate Player or Enemy statistics.
    /// </summary>
    public sealed class ExpeditionModifierDomainDirector
    {
        public ExpeditionModifierApplyResult ApplyModifier(
            ExpeditionModifierState modifierState,
            ExpeditionModifierDefinition definition,
            string sourceId,
            TimeSpan currentTime)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));
            if (definition == null)
                return ExpeditionModifierApplyResult.Rejected("Modifier definition is null.");

            if (!definition.TryValidate(out string validationError))
                return ExpeditionModifierApplyResult.Rejected(validationError);

            ExpeditionModifier existing = definition.StackingPolicy ==
                ExpeditionModifierStackingPolicy.Independent
                    ? null
                    : modifierState.FindByDefinitionId(definition.ContentId);

            if (existing != null && existing.IsTimed && existing.IsExpired(currentTime))
            {
                modifierState.RemoveByApplicationId(existing.ApplicationId);
                existing = null;
            }

            if (existing != null)
            {
                ExpeditionModifier updated = BuildUpdatedApplication(
                    existing, definition, currentTime);

                modifierState.Replace(existing, updated);
                return ExpeditionModifierApplyResult.Updated(updated);
            }

            ExpeditionModifier created = BuildNewApplication(
                definition, sourceId, currentTime);

            modifierState.Add(created);
            return ExpeditionModifierApplyResult.Added(created);
        }

        // Legacy API retained while existing producers are migrated in a later phase.
        public void AddModifier(
            ExpeditionModifierState modifierState,
            ExpeditionModifier modifier)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));
            modifierState.Add(modifier);
        }

        public bool RemoveModifier(
            ExpeditionModifierState modifierState,
            ExpeditionModifier modifier)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));

            return modifierState.RemoveByApplicationId(modifier.ApplicationId);
        }

        public bool RemoveModifier(
            ExpeditionModifierState modifierState,
            string applicationId)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));
            return modifierState.RemoveByApplicationId(applicationId);
        }

        public void RemoveExpiredModifiers(
            ExpeditionModifierState modifierState,
            TimeSpan currentTime)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));

            ExpeditionModifier[] snapshot = modifierState.Modifiers.ToArray();
            foreach (ExpeditionModifier modifier in snapshot)
            {
                if (!modifier.IsTimed)
                    continue;

                TimeSpan elapsed = currentTime - modifier.LastUpdatedAt;
                if (elapsed < TimeSpan.Zero)
                    elapsed = TimeSpan.Zero;

                TimeSpan remaining = modifier.RemainingDuration - elapsed;
                if (remaining <= TimeSpan.Zero)
                {
                    modifierState.RemoveByApplicationId(modifier.ApplicationId);
                    continue;
                }

                ExpeditionModifier updated = modifier.WithTiming(
                    remaining,
                    modifier.TotalGrantedDuration,
                    currentTime);

                modifierState.Replace(modifier, updated);
            }
        }

        public float GetTotalPercent(
            ExpeditionModifierState modifierState,
            ExpeditionModifierTarget target,
            string statId,
            TimeSpan currentTime)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));

            return modifierState.GetTotalPercent(target, statId);
        }

        public void Clear(ExpeditionModifierState modifierState)
        {
            if (modifierState == null)
                throw new ArgumentNullException(nameof(modifierState));

            modifierState.RemoveAll();
        }

        private static ExpeditionModifier BuildNewApplication(
            ExpeditionModifierDefinition definition,
            string sourceId,
            TimeSpan currentTime)
        {
            int initialStacks = 1;
            List<ExpeditionModifierEffect> effects = BuildEffects(definition, initialStacks);

            TimeSpan duration = definition.Lifetime == ExpeditionModifierLifetime.Timed
                ? definition.BaseDuration
                : TimeSpan.Zero;

            return new ExpeditionModifier(
                Guid.NewGuid().ToString("N"),
                definition.ContentId,
                sourceId,
                effects,
                definition.Lifetime,
                definition.StackingPolicy,
                initialStacks,
                definition.StackingPolicy == ExpeditionModifierStackingPolicy.Independent
                    ? 1
                    : definition.MaxStacks,
                duration,
                duration,
                duration,
                currentTime,
                currentTime);
        }

        private static ExpeditionModifier BuildUpdatedApplication(
            ExpeditionModifier existing,
            ExpeditionModifierDefinition definition,
            TimeSpan currentTime)
        {
            if (definition.StackingPolicy == ExpeditionModifierStackingPolicy.RefreshDuration)
            {
                return existing.WithTiming(
                    definition.BaseDuration,
                    definition.BaseDuration,
                    currentTime);
            }

            if (definition.StackingPolicy == ExpeditionModifierStackingPolicy.StackMagnitudeAndDuration)
            {
                int nextStacks = existing.StackCount;
                if (nextStacks < definition.MaxStacks)
                    nextStacks++;

                List<ExpeditionModifierEffect> effects =
                    BuildEffects(definition, nextStacks);

                TimeSpan elapsed = currentTime - existing.LastUpdatedAt;
                if (elapsed < TimeSpan.Zero)
                    elapsed = TimeSpan.Zero;

                TimeSpan remaining = existing.RemainingDuration - elapsed;
                if (remaining < TimeSpan.Zero)
                    remaining = TimeSpan.Zero;

                remaining += definition.BaseDuration;
                TimeSpan totalGranted =
                    existing.TotalGrantedDuration + definition.BaseDuration;

                return existing.WithStacksAndEffects(
                    nextStacks,
                    effects,
                    remaining,
                    totalGranted,
                    currentTime);
            }

            throw new InvalidOperationException(
                $"Stacking policy '{definition.StackingPolicy}' cannot update an existing application.");
        }

        private static List<ExpeditionModifierEffect> BuildEffects(
            ExpeditionModifierDefinition definition,
            int stackCount)
        {
            List<ExpeditionModifierEffect> result =
                new List<ExpeditionModifierEffect>();

            foreach (ExpeditionModifierEffectConfiguration effect in definition.Effects)
            {
                result.Add(new ExpeditionModifierEffect(
                    effect.Target,
                    effect.StatId,
                    effect.Operation,
                    effect.Value * stackCount));
            }

            return result;
        }
    }

    public enum ExpeditionModifierApplyStatus
    {
        Rejected,
        Added,
        Updated
    }

    public sealed class ExpeditionModifierApplyResult
    {
        public ExpeditionModifierApplyStatus Status { get; }
        public ExpeditionModifier Modifier { get; }
        public string Error { get; }
        public bool Succeeded => Status == ExpeditionModifierApplyStatus.Added ||
                                 Status == ExpeditionModifierApplyStatus.Updated;

        private ExpeditionModifierApplyResult(
            ExpeditionModifierApplyStatus status,
            ExpeditionModifier modifier,
            string error)
        {
            Status = status;
            Modifier = modifier;
            Error = error;
        }

        public static ExpeditionModifierApplyResult Rejected(string error)
        {
            return new ExpeditionModifierApplyResult(
                ExpeditionModifierApplyStatus.Rejected, null, error);
        }

        public static ExpeditionModifierApplyResult Added(ExpeditionModifier modifier)
        {
            return new ExpeditionModifierApplyResult(
                ExpeditionModifierApplyStatus.Added, modifier, null);
        }

        public static ExpeditionModifierApplyResult Updated(ExpeditionModifier modifier)
        {
            return new ExpeditionModifierApplyResult(
                ExpeditionModifierApplyStatus.Updated, modifier, null);
        }
    }
}
