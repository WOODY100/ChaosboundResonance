using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Items
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Items/Item Base Data",
        fileName = "ItemBaseData")]
    public sealed class ItemBaseData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string contentId;

        [SerializeField]
        private string displayName;

        [TextArea]
        [SerializeField]
        private string description;

        [SerializeField]
        private Sprite icon;

        [Header("Classification")]
        [SerializeField]
        private ItemCategory itemCategory = ItemCategory.Equipment;

        [SerializeField]
        private EquipmentType equipmentType = EquipmentType.None;

        [SerializeField]
        private ItemTier baseTier = ItemTier.Common;

        [Header("Equipment Stats")]
        [SerializeField]
        private List<EquipmentBaseStat> baseStats =
            new List<EquipmentBaseStat>();

        [Header("World Representation")]
        [SerializeField]
        private GameObject worldPrefab;

        public string ContentId =>
            contentId;

        public string DisplayName =>
            displayName;

        public string Description =>
            description;

        public Sprite Icon =>
            icon;

        public ItemTier BaseTier =>
            baseTier;

        public GameObject WorldPrefab =>
            worldPrefab;

        public ItemCategory Category =>
            itemCategory;

        public EquipmentType EquipmentType =>
            equipmentType;

        public IReadOnlyList<EquipmentBaseStat> BaseStats =>
            baseStats;

        private void OnValidate()
        {
            contentId =
                contentId != null
                    ? contentId.Trim()
                    : string.Empty;

            displayName =
                displayName != null
                    ? displayName.Trim()
                    : string.Empty;

            baseStats ??=
                new List<EquipmentBaseStat>();

            ValidateBaseStats();
            ValidateClassification();
        }

        private void ValidateBaseStats()
        {
            HashSet<StatType> registeredStats =
                new HashSet<StatType>();

            for (int i = 0; i < baseStats.Count; i++)
            {
                EquipmentBaseStat stat =
                    baseStats[i];

                if (!registeredStats.Add(stat.StatType))
                {
                    Debug.LogError(
                        $"{name}: Duplicate base Equipment Stat " +
                        $"for StatType '{stat.StatType}'.",
                        this);
                }
            }
        }

        private void ValidateClassification()
        {
            if (itemCategory == ItemCategory.Equipment &&
                equipmentType == EquipmentType.None)
            {
                Debug.LogWarning(
                    $"{name}: Item is classified as Equipment " +
                    "but EquipmentType is None.",
                    this);
            }

            if (itemCategory != ItemCategory.Equipment &&
                baseStats.Count > 0)
            {
                Debug.LogWarning(
                    $"{name}: Non-Equipment item contains " +
                    "Equipment Base Stats.",
                    this);
            }
        }
    }
}