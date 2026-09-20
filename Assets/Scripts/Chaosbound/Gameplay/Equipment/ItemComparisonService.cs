using System;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class ItemComparisonService
    {
        private readonly ItemContentResolver contentResolver;
        private readonly EquipmentStatResolver statResolver;

        public ItemComparisonService(
            ItemContentResolver contentResolver,
            EquipmentStatResolver statResolver)
        {
            this.contentResolver =
                contentResolver ??
                throw new ArgumentNullException(
                    nameof(contentResolver));

            this.statResolver =
                statResolver ??
                throw new ArgumentNullException(
                    nameof(statResolver));
        }

        public bool TryCompare(
            ItemInstance candidateItem,
            ItemInstance equippedItem,
            out ItemComparisonResult result)
        {
            result = null;

            if (candidateItem == null)
                return false;

            if (equippedItem == null)
                return false;

            if (!contentResolver.TryResolve(
                    candidateItem.BaseDataId,
                    out ItemBaseData candidateBaseData))
            {
                return false;
            }

            if (candidateBaseData == null)
                return false;

            if (!contentResolver.TryResolve(
                    equippedItem.BaseDataId,
                    out ItemBaseData equippedBaseData))
            {
                return false;
            }

            if (equippedBaseData == null)
                return false;

            if (candidateBaseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            if (equippedBaseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            if (candidateBaseData.EquipmentType ==
                EquipmentType.None)
            {
                return false;
            }

            if (equippedBaseData.EquipmentType ==
                EquipmentType.None)
            {
                return false;
            }

            if (candidateBaseData.EquipmentType !=
                equippedBaseData.EquipmentType)
            {
                return false;
            }

            EquipmentStatSnapshot candidateSnapshot =
                BuildSnapshot(
                    candidateBaseData,
                    candidateItem);

            EquipmentStatSnapshot equippedSnapshot =
                BuildSnapshot(
                    equippedBaseData,
                    equippedItem);

            result =
                new ItemComparisonResult(
                    equippedItem,
                    candidateItem,
                    equippedBaseData,
                    candidateBaseData,
                    candidateBaseData.EquipmentType);

            AddComparisonStats(
                equippedSnapshot,
                candidateSnapshot,
                result);

            return true;
        }

        private EquipmentStatSnapshot BuildSnapshot(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            EquipmentStatSnapshot snapshot =
                new EquipmentStatSnapshot();

            Array statTypes =
                Enum.GetValues(
                    typeof(StatType));

            for (int i = 0;
                 i < statTypes.Length;
                 i++)
            {
                StatType statType =
                    (StatType)statTypes.GetValue(i);

                if (!statResolver.TryResolve(
                        baseData,
                        itemInstance,
                        statType,
                        out ModifierType modifierType,
                        out float value))
                {
                    continue;
                }

                snapshot.Set(
                    statType,
                    modifierType,
                    value);
            }

            return snapshot;
        }

        private void AddComparisonStats(
            EquipmentStatSnapshot equippedSnapshot,
            EquipmentStatSnapshot candidateSnapshot,
            ItemComparisonResult result)
        {
            Array statTypes =
                Enum.GetValues(
                    typeof(StatType));

            for (int i = 0;
                 i < statTypes.Length;
                 i++)
            {
                StatType statType =
                    (StatType)statTypes.GetValue(i);

                bool hasEquipped =
                    equippedSnapshot.TryGet(
                        statType,
                        out EquipmentStatValue equippedValue);

                bool hasCandidate =
                    candidateSnapshot.TryGet(
                        statType,
                        out EquipmentStatValue candidateValue);

                if (!hasEquipped &&
                    !hasCandidate)
                {
                    continue;
                }

                ModifierType equippedModifierType =
                    hasEquipped
                        ? equippedValue.ModifierType
                        : default;

                ModifierType candidateModifierType =
                    hasCandidate
                        ? candidateValue.ModifierType
                        : default;

                float equippedNumericValue =
                    hasEquipped
                        ? equippedValue.Value
                        : 0f;

                float candidateNumericValue =
                    hasCandidate
                        ? candidateValue.Value
                        : 0f;

                result.AddStat(
                    new EquipmentComparisonStat(
                        statType,
                        hasEquipped,
                        hasCandidate,
                        equippedModifierType,
                        candidateModifierType,
                        equippedNumericValue,
                        candidateNumericValue));
            }
        }
    }
}