using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Source of truth for active modifier applications in one expedition.
    /// Mutations are exposed to the domain director; notifications are raised
    /// only after the corresponding state change has completed.
    /// </summary>
    public sealed class ExpeditionModifierState
    {
        private readonly List<ExpeditionModifier> modifiers =
            new List<ExpeditionModifier>();
        private readonly ReadOnlyCollection<ExpeditionModifier> readOnlyModifiers;

        public IReadOnlyList<ExpeditionModifier> Modifiers => readOnlyModifiers;

        public event Action<ExpeditionModifier> ModifierAdded;
        public event Action<ExpeditionModifier, ExpeditionModifier> ModifierUpdated;
        public event Action<ExpeditionModifier> ModifierRemoved;
        public event Action<IReadOnlyList<ExpeditionModifier>> ModifiersCleared;

        public ExpeditionModifierState()
        {
            readOnlyModifiers = modifiers.AsReadOnly();
        }

        public ExpeditionModifier FindByApplicationId(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                return null;

            return modifiers.FirstOrDefault(
                item => string.Equals(item.ApplicationId, applicationId, StringComparison.Ordinal));
        }

        public ExpeditionModifier FindByDefinitionId(string definitionId)
        {
            if (string.IsNullOrWhiteSpace(definitionId))
                return null;

            return modifiers.FirstOrDefault(
                item => string.Equals(item.DefinitionId, definitionId, StringComparison.Ordinal));
        }

        public IReadOnlyList<ExpeditionModifier> FindAllByDefinitionId(string definitionId)
        {
            if (string.IsNullOrWhiteSpace(definitionId))
                return Array.Empty<ExpeditionModifier>();

            return modifiers.Where(
                item => string.Equals(item.DefinitionId, definitionId, StringComparison.Ordinal)).ToArray();
        }

        internal void Add(ExpeditionModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));
            if (FindByApplicationId(modifier.ApplicationId) != null)
                throw new InvalidOperationException(
                    $"ApplicationId '{modifier.ApplicationId}' already exists.");

            modifiers.Add(modifier);
            ModifierAdded?.Invoke(modifier);
        }

        internal bool Replace(ExpeditionModifier previous, ExpeditionModifier current)
        {
            if (previous == null)
                throw new ArgumentNullException(nameof(previous));
            if (current == null)
                throw new ArgumentNullException(nameof(current));

            int index = modifiers.FindIndex(
                item => string.Equals(item.ApplicationId, previous.ApplicationId, StringComparison.Ordinal));
            if (index < 0)
                return false;
            if (!string.Equals(previous.ApplicationId, current.ApplicationId, StringComparison.Ordinal))
                throw new InvalidOperationException("Replacing an application must preserve ApplicationId.");

            modifiers[index] = current;
            ModifierUpdated?.Invoke(previous, current);
            return true;
        }

        internal bool RemoveByApplicationId(string applicationId)
        {
            int index = modifiers.FindIndex(
                item => string.Equals(item.ApplicationId, applicationId, StringComparison.Ordinal));
            if (index < 0)
                return false;

            ExpeditionModifier removed = modifiers[index];
            modifiers.RemoveAt(index);
            ModifierRemoved?.Invoke(removed);
            return true;
        }

        internal IReadOnlyList<ExpeditionModifier> RemoveAll()
        {
            if (modifiers.Count == 0)
                return Array.Empty<ExpeditionModifier>();

            ExpeditionModifier[] removed = modifiers.ToArray();
            modifiers.Clear();
            ModifiersCleared?.Invoke(removed);
            return removed;
        }

        public float GetTotalPercent(
            ExpeditionModifierTarget target,
            string statId)
        {
            if (string.IsNullOrWhiteSpace(statId))
                throw new ArgumentException("StatId cannot be empty.", nameof(statId));

            float total = 0f;
            foreach (ExpeditionModifier modifier in modifiers)
            {
                foreach (ExpeditionModifierEffect effect in modifier.Effects)
                {
                    if (effect.Target == target &&
                        string.Equals(effect.StatId, statId, StringComparison.Ordinal) &&
                        effect.Operation == ModifierType.Percent)
                    {
                        total += effect.Value;
                    }
                }
            }

            return total;
        }

        // Compatibility overload: querying state is deliberately side-effect free.
        public float GetTotalPercent(
            ExpeditionModifierTarget target,
            string statId,
            TimeSpan currentTime)
        {
            return GetTotalPercent(target, statId);
        }

        // Legacy compatibility. Expiration is coordinated through the director.
        internal void RemoveExpired(TimeSpan currentTime)
        {
            string[] expired = modifiers
                .Where(item => item.IsExpired(currentTime))
                .Select(item => item.ApplicationId)
                .ToArray();

            foreach (string id in expired)
                RemoveByApplicationId(id);
        }
    }
}
