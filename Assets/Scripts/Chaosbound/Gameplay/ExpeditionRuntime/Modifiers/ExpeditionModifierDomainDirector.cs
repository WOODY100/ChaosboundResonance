using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Coordinates operations over the Expedition Modifier Domain.
    ///
    /// The director does not own modifier state.
    /// Modifier state is owned by ExpeditionRuntimeState.
    /// </summary>
    public sealed class ExpeditionModifierDomainDirector
    {
        /// <summary>
        /// Adds a modifier to the supplied expedition modifier state.
        /// </summary>
        public void AddModifier(
            ExpeditionModifierState modifierState,
            ExpeditionModifier modifier)
        {
            if (modifierState == null)
            {
                throw new ArgumentNullException(
                    nameof(modifierState));
            }

            modifierState.Add(
                modifier);
        }

        /// <summary>
        /// Removes a modifier from the supplied expedition modifier state.
        /// </summary>
        public bool RemoveModifier(
            ExpeditionModifierState modifierState,
            ExpeditionModifier modifier)
        {
            if (modifierState == null)
            {
                throw new ArgumentNullException(
                    nameof(modifierState));
            }

            return modifierState.Remove(
                modifier);
        }

        /// <summary>
        /// Removes all expired timed modifiers.
        /// </summary>
        public void RemoveExpiredModifiers(
            ExpeditionModifierState modifierState,
            TimeSpan currentTime)
        {
            if (modifierState == null)
            {
                throw new ArgumentNullException(
                    nameof(modifierState));
            }

            modifierState.RemoveExpired(
                currentTime);
        }

        /// <summary>
        /// Gets the accumulated percentage for a target and stat.
        /// </summary>
        public float GetTotalPercent(
            ExpeditionModifierState modifierState,
            ExpeditionModifierTarget target,
            string statId,
            TimeSpan currentTime)
        {
            if (modifierState == null)
            {
                throw new ArgumentNullException(
                    nameof(modifierState));
            }

            return modifierState.GetTotalPercent(
                target,
                statId,
                currentTime);
        }

        /// <summary>
        /// Clears all expedition modifiers.
        /// </summary>
        public void Clear(
            ExpeditionModifierState modifierState)
        {
            if (modifierState == null)
            {
                throw new ArgumentNullException(
                    nameof(modifierState));
            }

            modifierState.Clear();
        }
    }
}