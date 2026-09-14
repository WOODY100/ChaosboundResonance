using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.ExpeditionRuntime.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Inventory.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class MaterialsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform slotContainer;

        [SerializeField]
        private MaterialSlotUI slotPrefab;

        private MaterialSlotUI[] slotUIs;

        private ExpeditionMaterialsState materialsState;
        private MaterialResolver materialResolver;

        private void Awake()
        {
            CreateInitialSlots();
        }

        private void Update()
        {
            ResolveDependencies();

            if (materialsState == null)
                return;

            Refresh();
        }

        private void ResolveDependencies()
        {
            if (materialsState == null)
            {
                if (RunManager.Instance == null)
                    return;

                ExpeditionRuntimeState runtimeState =
                    RunManager.Instance.ExpeditionRuntimeState;

                if (runtimeState == null)
                    return;

                materialsState =
                    runtimeState.Materials;
            }

            if (materialResolver == null)
            {
                ExpeditionRuntimeCompositionContext context =
                    ExpeditionRuntimeCompositionContext.Current;

                if (context == null)
                    return;

                if (context.MaterialDatabase == null)
                    return;

                materialResolver =
                    new MaterialResolver(
                        context.MaterialDatabase);
            }
        }

        private void CreateInitialSlots()
        {
            if (slotContainer == null)
                return;

            if (slotPrefab == null)
                return;

            slotUIs =
                new MaterialSlotUI[
                    InventoryConstants.MainSlotCount];

            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i] =
                    Instantiate(
                        slotPrefab,
                        slotContainer);
            }
        }

        private void Refresh()
        {
            IReadOnlyDictionary<string, int> amounts =
                materialsState.GetAmounts();

            EnsureSlotCapacity(
                amounts.Count);

            List<MaterialDisplayEntry> entries =
                BuildSortedEntries(amounts);

            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (i < entries.Count)
                {
                    MaterialDisplayEntry entry =
                        entries[i];

                    slotUIs[i].SetMaterial(
                        entry.ContentId,
                        entry.Definition.Icon,
                        entry.Amount);
                }
                else
                {
                    slotUIs[i].Clear();
                }
            }
        }

        private List<MaterialDisplayEntry> BuildSortedEntries(
            IReadOnlyDictionary<string, int> amounts)
        {
            List<MaterialDisplayEntry> entries =
                new List<MaterialDisplayEntry>();

            foreach (
                KeyValuePair<string, int> pair
                in amounts)
            {
                MaterialDefinition definition;

                if (!materialResolver.TryResolve(
                        pair.Key,
                        out definition))
                {
                    continue;
                }

                entries.Add(
                    new MaterialDisplayEntry(
                        pair.Key,
                        definition,
                        pair.Value));
            }

            entries.Sort(
                CompareDisplayNames);

            return entries;
        }

        private void EnsureSlotCapacity(
            int requiredCount)
        {
            int targetCount =
                Mathf.Max(
                    InventoryConstants.MainSlotCount,
                    requiredCount);

            if (targetCount <= slotUIs.Length)
                return;

            int currentCount =
                slotUIs.Length;

            int newCount =
                ((targetCount + 4) / 5) * 5;

            Array.Resize(
                ref slotUIs,
                newCount);

            for (int i = currentCount; i < newCount; i++)
            {
                slotUIs[i] =
                    Instantiate(
                        slotPrefab,
                        slotContainer);
            }
        }

        private static int CompareDisplayNames(
            MaterialDisplayEntry a,
            MaterialDisplayEntry b)
        {
            return string.Compare(
                a.Definition.DisplayName,
                b.Definition.DisplayName,
                StringComparison.OrdinalIgnoreCase);
        }

        private sealed class MaterialDisplayEntry
        {
            public string ContentId { get; }

            public MaterialDefinition Definition { get; }

            public int Amount { get; }

            public MaterialDisplayEntry(
                string contentId,
                MaterialDefinition definition,
                int amount)
            {
                ContentId = contentId;
                Definition = definition;
                Amount = amount;
            }
        }
    }
}