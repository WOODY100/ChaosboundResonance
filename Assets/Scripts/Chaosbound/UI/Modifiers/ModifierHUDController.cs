using Chaosbound.Content.ExpeditionModifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.UI.Modifiers
{
    /// <summary>
    /// Presents active expedition modifier applications.
    /// State and domain remain authoritative for stacking and duration.
    /// </summary>
    public sealed class ModifierHUDController : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField]
        private ExpeditionModifierDefinitionDatabase definitionDatabase;

        [Header("Layout")]
        [SerializeField]
        private RectTransform iconContainer;

        [SerializeField]
        private ModifierHUDIconView iconPrefab;

        private readonly Dictionary<string, ModifierHUDIconView> activeViews =
            new Dictionary<string, ModifierHUDIconView>(StringComparer.Ordinal);

        private ExpeditionModifierState modifierState;
        private TimeSpan elapsedTime;
        private bool isInitialized;

        public int ActiveIconCount => activeViews.Count;

        public void Initialize(
            ExpeditionModifierState state,
            TimeSpan currentElapsedTime)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            ValidateConfiguration();

            Unsubscribe();
            ClearViews();

            modifierState = state;
            elapsedTime = currentElapsedTime;

            modifierState.ModifierAdded += HandleModifierAdded;
            modifierState.ModifierUpdated += HandleModifierUpdated;
            modifierState.ModifierRemoved += HandleModifierRemoved;
            modifierState.ModifiersCleared += HandleModifiersCleared;

            isInitialized = true;

            for (int i = 0; i < modifierState.Modifiers.Count; i++)
                AddOrRefreshView(modifierState.Modifiers[i]);
        }

        public void Tick(TimeSpan currentElapsedTime)
        {
            if (!isInitialized)
                return;

            elapsedTime = currentElapsedTime;

            foreach (KeyValuePair<string, ModifierHUDIconView> pair in activeViews)
            {
                ExpeditionModifier modifier =
                    modifierState.FindByApplicationId(pair.Key);

                if (modifier != null && pair.Value != null)
                    pair.Value.Refresh(modifier, elapsedTime);
            }
        }

        public void Cleanup()
        {
            Unsubscribe();
            ClearViews();
            modifierState = null;
            isInitialized = false;
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void ValidateConfiguration()
        {
            if (definitionDatabase == null)
                throw new InvalidOperationException(
                    $"{nameof(ModifierHUDController)} requires an ExpeditionModifierDefinitionDatabase.");

            if (iconContainer == null)
                throw new InvalidOperationException(
                    $"{nameof(ModifierHUDController)} requires an Icon Container.");

            if (iconPrefab == null)
                throw new InvalidOperationException(
                    $"{nameof(ModifierHUDController)} requires an Icon Prefab.");
        }

        private void HandleModifierAdded(ExpeditionModifier modifier)
        {
            AddOrRefreshView(modifier);
        }

        private void HandleModifierUpdated(
            ExpeditionModifier previous,
            ExpeditionModifier current)
        {
            AddOrRefreshView(current);
        }

        private void HandleModifierRemoved(ExpeditionModifier modifier)
        {
            RemoveView(modifier.ApplicationId);
        }

        private void HandleModifiersCleared(
            IReadOnlyList<ExpeditionModifier> removedModifiers)
        {
            if (removedModifiers == null)
                return;

            for (int i = 0; i < removedModifiers.Count; i++)
                RemoveView(removedModifiers[i].ApplicationId);
        }

        private void AddOrRefreshView(ExpeditionModifier modifier)
        {
            if (modifier == null)
                return;

            if (!definitionDatabase.TryGet(
                    modifier.DefinitionId,
                    out ExpeditionModifierDefinition definition))
            {
                Debug.LogError(
                    $"[ModifierHUDController] Definition '{modifier.DefinitionId}' " +
                    "was not found in the assigned database.",
                    this);
                return;
            }

            if (!activeViews.TryGetValue(
                    modifier.ApplicationId,
                    out ModifierHUDIconView view) ||
                view == null)
            {
                view = Instantiate(iconPrefab, iconContainer);
                activeViews[modifier.ApplicationId] = view;
            }

            view.Bind(modifier, definition.Icon, elapsedTime);
        }

        private void RemoveView(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                return;

            if (!activeViews.TryGetValue(applicationId, out ModifierHUDIconView view))
                return;

            activeViews.Remove(applicationId);

            if (view != null)
                Destroy(view.gameObject);
        }

        private void Unsubscribe()
        {
            if (modifierState == null)
                return;

            modifierState.ModifierAdded -= HandleModifierAdded;
            modifierState.ModifierUpdated -= HandleModifierUpdated;
            modifierState.ModifierRemoved -= HandleModifierRemoved;
            modifierState.ModifiersCleared -= HandleModifiersCleared;
        }

        private void ClearViews()
        {
            foreach (ModifierHUDIconView view in activeViews.Values)
            {
                if (view != null)
                    Destroy(view.gameObject);
            }

            activeViews.Clear();
        }
    }
}
