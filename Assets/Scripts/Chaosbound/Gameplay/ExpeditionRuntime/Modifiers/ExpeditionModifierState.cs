using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Stores the active modifiers of the current expedition.
    ///
    /// This class owns modifier state only.
    /// It does not know how individual gameplay domains
    /// apply modifiers to their statistics.
    /// </summary>
    public sealed class ExpeditionModifierState
    {
        private readonly List<ExpeditionModifier>
            modifiers;

        /// <summary>
        /// Gets the currently active expedition modifiers.
        /// </summary>
        public IReadOnlyList<ExpeditionModifier> Modifiers =>
            modifiers;

        /// <summary>
        /// Creates a new expedition modifier state.
        /// </summary>
        public ExpeditionModifierState()
        {
            modifiers =
                new List<ExpeditionModifier>();
        }

        /// <summary>
        /// Adds a modifier to the current expedition state.
        ///
        /// Each addition represents an independent application
        /// of the modifier and therefore supports stacking.
        /// </summary>
        public void Add(
            ExpeditionModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(
                    nameof(modifier));
            }

            modifiers.Add(modifier);
        }

        /// <summary>
        /// Removes the specified modifier from the current state.
        /// </summary>
        /// <returns>
        /// True when the modifier was present and removed.
        /// </returns>
        public bool Remove(
            ExpeditionModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(
                    nameof(modifier));
            }

            return modifiers.Remove(modifier);
        }

        /// <summary>
        /// Gets the total additive percentage for the specified
        /// target and stat.
        ///
        /// Percentages are summed directly:
        ///
        /// +20% +30% -10% = +40%
        ///
        /// The result is intended to be applied against the
        /// stat base value by the consuming runtime system.
        /// </summary>
        public float GetTotalPercent(
            ExpeditionModifierTarget target,
            string statId,
            TimeSpan currentTime)
        {
            ValidateStatId(statId);

            RemoveExpired(currentTime);

            float totalPercent = 0f;

            foreach (ExpeditionModifier modifier in modifiers)
            {
                foreach (ExpeditionModifierEffect effect
                    in modifier.Effects)
                {
                    if (effect.Target != target)
                        continue;

                    if (!string.Equals(
                        effect.StatId,
                        statId,
                        StringComparison.Ordinal))
                    {
                        continue;
                    }

                    totalPercent += effect.Percent;
                }
            }

            return totalPercent;
        }

        /// <summary>
        /// Removes all timed modifiers that have expired.
        /// </summary>
        public void RemoveExpired(
            TimeSpan currentTime)
        {
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                ExpeditionModifier modifier =
                    modifiers[i];

                if (modifier.IsExpired(currentTime))
                {
                    modifiers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clears all active modifiers.
        /// </summary>
        public void Clear()
        {
            modifiers.Clear();
        }

        private static void ValidateStatId(
            string statId)
        {
            if (string.IsNullOrWhiteSpace(statId))
            {
                throw new ArgumentException(
                    "StatId cannot be empty.",
                    nameof(statId));
            }
        }
    }
}