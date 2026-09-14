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

        public IReadOnlyList<MaterialDefinition> Materials => materials;

        public bool TryGet(
            string contentId,
            out MaterialDefinition definition)
        {
            definition = null;

            if (string.IsNullOrEmpty(contentId))
                return false;

            for (int i = 0; i < materials.Count; i++)
            {
                MaterialDefinition candidate = materials[i];

                if (candidate == null)
                    continue;

                if (candidate.ContentId == contentId)
                {
                    definition = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}