using System;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveEquipmentRolledStatData
    {
        public StatType StatType;
        public ModifierType ModifierType;
        public float RolledValue;
    }
}