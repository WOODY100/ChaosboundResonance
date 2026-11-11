using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;

namespace Chaosbound.Content.ExpeditionModifiers
{
    [CreateAssetMenu(
        fileName = "ExpeditionModifierDefinitionDatabase",
        menuName = "Chaosbound/Expedition/Modifier Definition Database")]
    public sealed class ExpeditionModifierDefinitionDatabase : ScriptableObject
    {
        [SerializeField]
        private List<ExpeditionModifierDefinition> definitions =
            new List<ExpeditionModifierDefinition>();

        private Dictionary<string, ExpeditionModifierDefinition> definitionsById;
        private HashSet<string> ambiguousIds;

        public IReadOnlyList<ExpeditionModifierDefinition> Definitions =>
            definitions;

        public bool TryGet(
            string contentId,
            out ExpeditionModifierDefinition definition)
        {
            definition = null;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            EnsureIndex();

            contentId = contentId.Trim();

            if (ambiguousIds.Contains(contentId))
                return false;

            return definitionsById.TryGetValue(contentId, out definition);
        }

        private void OnEnable()
        {
            RebuildIndex();
        }

        private void EnsureIndex()
        {
            if (definitionsById == null || ambiguousIds == null)
                RebuildIndex();
        }

        private void RebuildIndex()
        {
            definitionsById =
                new Dictionary<string, ExpeditionModifierDefinition>(
                    StringComparer.Ordinal);

            ambiguousIds =
                new HashSet<string>(StringComparer.Ordinal);

            if (definitions == null)
                return;

            for (int i = 0; i < definitions.Count; i++)
            {
                ExpeditionModifierDefinition definition = definitions[i];
                if (definition == null ||
                    string.IsNullOrWhiteSpace(definition.ContentId))
                    continue;

                string id = definition.ContentId.Trim();

                if (ambiguousIds.Contains(id))
                    continue;

                if (definitionsById.ContainsKey(id))
                {
                    definitionsById.Remove(id);
                    ambiguousIds.Add(id);
                    continue;
                }

                definitionsById.Add(id, definition);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildIndex();

            if (definitions == null)
                return;

            HashSet<string> ids = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < definitions.Count; i++)
            {
                ExpeditionModifierDefinition definition = definitions[i];
                if (definition == null ||
                    string.IsNullOrWhiteSpace(definition.ContentId))
                    continue;

                if (!ids.Add(definition.ContentId.Trim()))
                {
                    Debug.LogError(
                        $"Duplicate Expedition Modifier Definition ContentId '{definition.ContentId}' in '{name}'.",
                        this);
                }
            }
        }
#endif
    }
}
