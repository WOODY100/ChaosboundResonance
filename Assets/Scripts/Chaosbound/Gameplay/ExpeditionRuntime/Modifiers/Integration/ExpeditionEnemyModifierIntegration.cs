using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System.Collections.Generic;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Integration
{
    /// <summary>
    /// Keeps one active enemy synchronized with Enemy-targeted expedition
    /// modifier changes. The expedition state remains the source of truth.
    /// </summary>
    public sealed class ExpeditionEnemyModifierIntegration : IDisposable
    {
        private readonly ExpeditionModifierState modifierState;
        private readonly EnemyRuntimeStats runtimeStats;
        private readonly EnemyHealth health;
        private readonly EnemyRuntimeNavigation navigation;
        private bool isDisposed;

        public ExpeditionEnemyModifierIntegration(
            ExpeditionModifierState modifierState,
            EnemyRuntimeStats runtimeStats,
            EnemyHealth health,
            EnemyRuntimeNavigation navigation)
        {
            this.modifierState = modifierState
                ?? throw new ArgumentNullException(nameof(modifierState));
            this.runtimeStats = runtimeStats
                ?? throw new ArgumentNullException(nameof(runtimeStats));
            this.health = health
                ?? throw new ArgumentNullException(nameof(health));
            this.navigation = navigation
                ?? throw new ArgumentNullException(nameof(navigation));

            modifierState.ModifierAdded += HandleModifierChanged;
            modifierState.ModifierUpdated += HandleModifierUpdated;
            modifierState.ModifierRemoved += HandleModifierChanged;
            modifierState.ModifiersCleared += HandleModifiersCleared;

            RefreshEnemy();
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            modifierState.ModifierAdded -= HandleModifierChanged;
            modifierState.ModifierUpdated -= HandleModifierUpdated;
            modifierState.ModifierRemoved -= HandleModifierChanged;
            modifierState.ModifiersCleared -= HandleModifiersCleared;
        }

        private void HandleModifierChanged(ExpeditionModifier modifier)
        {
            if (ContainsEnemyEffect(modifier))
                RefreshEnemy();
        }

        private void HandleModifierUpdated(
            ExpeditionModifier previous,
            ExpeditionModifier current)
        {
            if (ContainsEnemyEffect(previous) || ContainsEnemyEffect(current))
                RefreshEnemy();
        }

        private void HandleModifiersCleared(
            IReadOnlyList<ExpeditionModifier> removed)
        {
            if (removed == null)
                return;

            for (int i = 0; i < removed.Count; i++)
            {
                if (!ContainsEnemyEffect(removed[i]))
                    continue;

                RefreshEnemy();
                return;
            }
        }

        private static bool ContainsEnemyEffect(ExpeditionModifier modifier)
        {
            if (modifier == null)
                return false;

            for (int i = 0; i < modifier.Effects.Count; i++)
            {
                if (modifier.Effects[i].Target == ExpeditionModifierTarget.Enemy)
                    return true;
            }

            return false;
        }

        private void RefreshEnemy()
        {
            if (isDisposed || !runtimeStats.IsInitialized)
                return;

            runtimeStats.Refresh();
            health.RefreshMaxHealth(runtimeStats.MaxHealth);
            navigation.RefreshMoveSpeed(runtimeStats.MoveSpeed);
        }
    }
}
