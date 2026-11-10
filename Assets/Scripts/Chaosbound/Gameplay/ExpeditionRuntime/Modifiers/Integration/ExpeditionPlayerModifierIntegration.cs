using Player.Enums;
using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Integration
{
    /// <summary>
    /// Translates Player-targeted expedition modifier applications into
    /// temporary PlayerModifierSystem sources. It owns no modifier duration
    /// or stacking rules.
    /// </summary>
    public sealed class ExpeditionPlayerModifierIntegration : IDisposable
    {
        private const string SourcePrefix = "ExpeditionModifier:";

        private readonly ExpeditionModifierState modifierState;
        private readonly PlayerModifierSystem playerModifierSystem;
        private readonly HashSet<string> registeredApplicationIds =
            new HashSet<string>();

        private bool isDisposed;

        public ExpeditionPlayerModifierIntegration(
            ExpeditionModifierState modifierState,
            PlayerModifierSystem playerModifierSystem)
        {
            this.modifierState = modifierState
                ?? throw new ArgumentNullException(nameof(modifierState));
            this.playerModifierSystem = playerModifierSystem
                ?? throw new ArgumentNullException(nameof(playerModifierSystem));

            modifierState.ModifierAdded += HandleModifierAdded;
            modifierState.ModifierUpdated += HandleModifierUpdated;
            modifierState.ModifierRemoved += HandleModifierRemoved;
            modifierState.ModifiersCleared += HandleModifiersCleared;

            // Support composition created after applications already exist.
            foreach (ExpeditionModifier modifier in modifierState.Modifiers)
                RegisterOrUpdate(modifier);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            modifierState.ModifierAdded -= HandleModifierAdded;
            modifierState.ModifierUpdated -= HandleModifierUpdated;
            modifierState.ModifierRemoved -= HandleModifierRemoved;
            modifierState.ModifiersCleared -= HandleModifiersCleared;

            RemoveAllOwnedSources();
        }

        private void HandleModifierAdded(ExpeditionModifier modifier)
        {
            RegisterOrUpdate(modifier);
        }

        private void HandleModifierUpdated(
            ExpeditionModifier previous,
            ExpeditionModifier current)
        {
            RegisterOrUpdate(current);
        }

        private void HandleModifierRemoved(ExpeditionModifier modifier)
        {
            RemoveOwnedSource(modifier.ApplicationId);
        }

        private void HandleModifiersCleared(
            IReadOnlyList<ExpeditionModifier> removedModifiers)
        {
            if (removedModifiers == null)
                return;

            foreach (ExpeditionModifier modifier in removedModifiers)
                RemoveOwnedSource(modifier.ApplicationId);
        }

        private void RegisterOrUpdate(ExpeditionModifier modifier)
        {
            if (modifier == null || isDisposed)
                return;

            List<StatModifier> converted = new List<StatModifier>();

            foreach (ExpeditionModifierEffect effect in modifier.Effects)
            {
                if (effect.Target != ExpeditionModifierTarget.Player)
                    continue;

                if (!TryMapStat(effect.StatId, out StatType statType))
                {
                    Debug.LogError(
                        $"[ExpeditionPlayerModifierIntegration] Unsupported Player StatId '{effect.StatId}' " +
                        $"in modifier '{modifier.DefinitionId}'. Application '{modifier.ApplicationId}' was not registered.");
                    return;
                }

                if (effect.Operation != ModifierType.Percent)
                {
                    Debug.LogError(
                        $"[ExpeditionPlayerModifierIntegration] Unsupported operation '{effect.Operation}' " +
                        $"in modifier '{modifier.DefinitionId}'.");
                    return;
                }

                // Expedition values are fractional (0.20 = +20%);
                // PlayerModifierSystem.Percent values are percentage points (20 = +20%).
                converted.Add(new StatModifier
                {
                    StatType = statType,
                    ModifierType = effect.Operation,
                    Value = effect.Value * 100f
                });
            }

            if (converted.Count == 0)
            {
                RemoveOwnedSource(modifier.ApplicationId);
                return;
            }

            string sourceId = BuildSourceId(modifier.ApplicationId);
            ModifierSource source = new ModifierSource(sourceId)
            {
                Modifiers = converted
            };

            playerModifierSystem.AddSource(
                ModifierLayer.Temporary,
                source);

            registeredApplicationIds.Add(modifier.ApplicationId);
        }

        private void RemoveOwnedSource(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                return;

            if (!registeredApplicationIds.Remove(applicationId))
                return;

            playerModifierSystem.RemoveSource(
                ModifierLayer.Temporary,
                BuildSourceId(applicationId));
        }

        private void RemoveAllOwnedSources()
        {
            if (registeredApplicationIds.Count == 0)
                return;

            string[] applicationIds =
                new string[registeredApplicationIds.Count];
            registeredApplicationIds.CopyTo(applicationIds);
            registeredApplicationIds.Clear();

            foreach (string applicationId in applicationIds)
            {
                playerModifierSystem.RemoveSource(
                    ModifierLayer.Temporary,
                    BuildSourceId(applicationId));
            }
        }

        private static string BuildSourceId(string applicationId)
        {
            return SourcePrefix + applicationId;
        }

        private static bool TryMapStat(
            string statId,
            out StatType statType)
        {
            switch (statId)
            {
                case "Damage":
                    statType = StatType.Damage;
                    return true;

                case "AttackSpeed":
                    statType = StatType.AttackSpeed;
                    return true;

                case "MoveSpeed":
                case "MovementSpeed":
                    statType = StatType.MovementSpeed;
                    return true;

                case "MaxHealth":
                case "MaxHP":
                    statType = StatType.MaxHP;
                    return true;

                case "HPRegen":
                    statType = StatType.HPRegen;
                    return true;

                default:
                    statType = default(StatType);
                    return false;
            }
        }
    }
}
