using Chaosbound.Content.Materials;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersistentMaterialsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform slotContainer;

        [SerializeField]
        private MaterialSlotUI slotPrefab;

        [Header("Layout")]
        [SerializeField]
        private int minimumSlotCount = 20;

        [SerializeField]
        private int slotsPerExpansion = 5;

        private MaterialSlotUI[] slotUIs;

        private PersistentMaterialsState materialsState;
        private PersistentMaterialSeenState materialSeenState;
        private MaterialResolver materialResolver;

        private void Awake()
        {
            CreateInitialSlots();
        }

        private void Update()
        {
            ResolveDependencies();

            if (materialsState == null ||
                materialSeenState == null ||
                materialResolver == null)
            {
                return;
            }

            Refresh();
        }

        private void ResolveDependencies()
        {
            if (materialsState == null ||
                materialSeenState == null)
            {
                BootstrapContext bootstrapContext =
                    BootstrapContext.Current;

                if (bootstrapContext == null)
                    return;

                PersistentInventoryRuntime inventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

                if (inventoryRuntime == null)
                    return;

                PersistentInventoryState state =
                    inventoryRuntime.State;

                if (state == null)
                    return;

                materialsState =
                    state.Materials;

                materialSeenState =
                    state.MaterialSeenState;
            }

            if (materialResolver == null)
            {
                GameContentContext contentContext =
                    GameContentContext.Current;

                if (contentContext == null)
                    return;

                if (contentContext.MaterialDatabase == null)
                    return;

                materialResolver =
                    new MaterialResolver(
                        contentContext.MaterialDatabase);
            }
        }

        private void CreateInitialSlots()
        {
            if (slotContainer == null)
                return;

            if (slotPrefab == null)
                return;

            minimumSlotCount =
                Mathf.Max(
                    0,
                    minimumSlotCount);

            slotsPerExpansion =
                Mathf.Max(
                    1,
                    slotsPerExpansion);

            slotUIs =
                new MaterialSlotUI[
                    minimumSlotCount];

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

                    bool isNew =
                        materialSeenState.IsNew(
                            entry.ContentId);

                    slotUIs[i].SetNewIndicator(
                        isNew);
                }
                else
                {
                    slotUIs[i].Clear();
                }
            }
        }

        private List<MaterialDisplayEntry>
            BuildSortedEntries(
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
                    minimumSlotCount,
                    requiredCount);

            if (targetCount <= slotUIs.Length)
                return;

            int currentCount =
                slotUIs.Length;

            int remainder =
                targetCount %
                slotsPerExpansion;

            if (remainder != 0)
            {
                targetCount +=
                    slotsPerExpansion -
                    remainder;
            }

            Array.Resize(
                ref slotUIs,
                targetCount);

            for (
                int i = currentCount;
                i < targetCount;
                i++)
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