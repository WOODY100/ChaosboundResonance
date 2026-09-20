using System;
using UnityEngine;

namespace Chaosbound.Content.Items
{
    [Serializable]
    public struct EquipmentBaseStat
    {
        [SerializeField]
        private StatType statType;

        [SerializeField]
        private ModifierType modifierType;

        [SerializeField]
        private float value;

        public StatType StatType =>
            statType;

        public ModifierType ModifierType =>
            modifierType;

        public float Value =>
            value;
    }
}