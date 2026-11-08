using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Materials
{
    [CreateAssetMenu(
        fileName = "MaterialDatabase",
        menuName = "Chaosbound/Materials/Material Database")]
    public sealed class MaterialDatabase : ScriptableObject
    {
        [SerializeField]
        private List<MaterialDefinition> materials =
            new List<MaterialDefinition>();

        private Dictionary<string, MaterialDefinition> materialsById;
        private HashSet<string> ambiguousIds;

        public IReadOnlyList<MaterialDefinition> Materials =>
            materials;

        private void OnEnable()
        {
            RebuildIndex();
        }

        public bool TryGet(
            string contentId,
            out MaterialDefinition definition)
        {
            definition = null;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            EnsureIndex();

            contentId = contentId.Trim();

            if (ambiguousIds.Contains(contentId))
                return false;

            return materialsById.TryGetValue(
                contentId,
                out definition);
        }

        private void EnsureIndex()
        {
            if (materialsById == null ||
                ambiguousIds == null)
            {
                RebuildIndex();
            }
        }

        private void RebuildIndex()
        {
            materialsById =
                new Dictionary<string, MaterialDefinition>(
                    StringComparer.Ordinal);

            ambiguousIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            if (materials == null)
                return;

            for (int i = 0; i < materials.Count; i++)
            {
                MaterialDefinition candidate =
                    materials[i];

                if (candidate == null)
                    continue;

                string contentId =
                    candidate.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                    continue;

                contentId = contentId.Trim();

                if (ambiguousIds.Contains(contentId))
                    continue;

                if (materialsById.ContainsKey(contentId))
                {
                    materialsById.Remove(contentId);
                    ambiguousIds.Add(contentId);

                    continue;
                }

                materialsById.Add(
                    contentId,
                    candidate);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            ValidateMaterials();
            RebuildIndex();
        }

        private void ValidateMaterials()
        {
            if (materials == null)
                return;

            HashSet<string> registeredIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0; i < materials.Count; i++)
            {
                MaterialDefinition candidate =
                    materials[i];

                if (candidate == null)
                {
                    Debug.LogWarning(
                        $"{nameof(MaterialDatabase)}: " +
                        $"Null material reference at index {i}.",
                        this);

                    continue;
                }

                string contentId =
                    candidate.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                {
                    Debug.LogError(
                        $"{nameof(MaterialDatabase)}: " +
                        $"Material '{candidate.name}' " +
                        "has an empty ContentId.",
                        candidate);

                    continue;
                }

                contentId = contentId.Trim();

                if (!registeredIds.Add(contentId))
                {
                    Debug.LogError(
                        $"{nameof(MaterialDatabase)}: " +
                        $"Duplicate ContentId '{contentId}'.",
                        this);
                }
            }
        }

#endif
    }
}