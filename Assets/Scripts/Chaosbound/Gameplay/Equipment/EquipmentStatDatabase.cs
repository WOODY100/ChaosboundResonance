using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Equipment/Equipment Stat Database",
        fileName = "EquipmentStatDatabase")]
    public sealed class EquipmentStatDatabase : ScriptableObject
    {
        [SerializeField]
        private List<EquipmentStatDefinition> definitions =
            new List<EquipmentStatDefinition>();

        public IReadOnlyList<EquipmentStatDefinition> Definitions =>
            definitions;

        public bool TryGetDefinition(
            StatType statType,
            out EquipmentStatDefinition definition)
        {
            for (int i = 0; i < definitions.Count; i++)
            {
                EquipmentStatDefinition candidate = definitions[i];

                if (candidate == null)
                    continue;

                if (candidate.StatType != statType)
                    continue;

                definition = candidate;
                return true;
            }

            definition = null;
            return false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (definitions == null)
                definitions =
                    new List<EquipmentStatDefinition>();

            HashSet<StatType> registeredStats =
                new HashSet<StatType>();

            for (int i = 0; i < definitions.Count; i++)
            {
                EquipmentStatDefinition definition =
                    definitions[i];

                if (definition == null)
                    continue;

                if (!registeredStats.Add(
                    definition.StatType))
                {
                    Debug.LogError(
                        $"{name}: Duplicate Equipment Stat Definition " +
                        $"for StatType '{definition.StatType}'.",
                        this);
                }
            }
        }
#endif
    }
}