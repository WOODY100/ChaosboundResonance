using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    [Serializable]
    public struct EquipmentStatOption
    {
        [SerializeField]
        private StatType statType;

        [SerializeField]
        private ModifierType modifierType;

        [SerializeField]
        private float rolledValue;

        public StatType StatType =>
            statType;

        public ModifierType ModifierType =>
            modifierType;

        public float RolledValue =>
            rolledValue;

        public EquipmentStatOption(
            StatType statType,
            ModifierType modifierType,
            float rolledValue)
        {
            this.statType = statType;
            this.modifierType = modifierType;
            this.rolledValue = rolledValue;
        }

        public EquipmentRolledStat ToRolledStat()
        {
            return new EquipmentRolledStat(
                statType,
                modifierType,
                rolledValue);
        }
    }
}