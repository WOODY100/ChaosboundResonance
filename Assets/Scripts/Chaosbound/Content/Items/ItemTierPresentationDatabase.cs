using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Items
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Items/Item Tier Presentation Database",
        fileName = "ItemTierPresentationDatabase")]
    public sealed class ItemTierPresentationDatabase : ScriptableObject
    {
        [SerializeField]
        private List<ItemTierPresentationDefinition> definitions = new();

        private Dictionary<ItemTier, ItemTierPresentationDefinition>
            definitionsByTier;

        public bool TryGet(
            ItemTier tier,
            out ItemTierPresentationDefinition definition)
        {
            BuildLookupIfNeeded();

            return definitionsByTier.TryGetValue(
                tier,
                out definition);
        }

        private void BuildLookupIfNeeded()
        {
            if (definitionsByTier != null)
                return;

            definitionsByTier =
                new Dictionary<ItemTier, ItemTierPresentationDefinition>();

            for (int i = 0; i < definitions.Count; i++)
            {
                ItemTierPresentationDefinition definition =
                    definitions[i];

                if (definition == null)
                    continue;

                if (definitionsByTier.ContainsKey(definition.Tier))
                {
                    Debug.LogError(
                        $"Duplicate presentation definition for " +
                        $"ItemTier '{definition.Tier}' in database '{name}'.",
                        this);

                    continue;
                }

                definitionsByTier.Add(
                    definition.Tier,
                    definition);
            }
        }

        private void OnValidate()
        {
            definitionsByTier = null;

            HashSet<ItemTier> tiers =
                new HashSet<ItemTier>();

            for (int i = 0; i < definitions.Count; i++)
            {
                ItemTierPresentationDefinition definition =
                    definitions[i];

                if (definition == null)
                    continue;

                if (!tiers.Add(definition.Tier))
                {
                    Debug.LogError(
                        $"Duplicate presentation definition for " +
                        $"ItemTier '{definition.Tier}' in database '{name}'.",
                        this);
                }
            }
        }
    }
}