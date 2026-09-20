namespace Chaosbound.Gameplay.Equipment
{
    public readonly struct EquipmentComparisonDisplayEntry
    {
        public StatType StatType { get; }

        public bool HasEquippedValue { get; }
        public bool HasCandidateValue { get; }

        public ModifierType EquippedModifierType { get; }
        public ModifierType CandidateModifierType { get; }

        public float EquippedValue { get; }
        public float CandidateValue { get; }

        public bool HasDifference { get; }
        public float Difference { get; }

        public EquipmentComparisonDisplayState State { get; }

        public bool IsDirectlyComparable =>
            HasDifference;

        public EquipmentComparisonDisplayEntry(
            StatType statType,
            bool hasEquippedValue,
            bool hasCandidateValue,
            ModifierType equippedModifierType,
            ModifierType candidateModifierType,
            float equippedValue,
            float candidateValue,
            bool hasDifference,
            float difference,
            EquipmentComparisonDisplayState state)
        {
            StatType = statType;

            HasEquippedValue = hasEquippedValue;
            HasCandidateValue = hasCandidateValue;

            EquippedModifierType = equippedModifierType;
            CandidateModifierType = candidateModifierType;

            EquippedValue = equippedValue;
            CandidateValue = candidateValue;

            HasDifference = hasDifference;
            Difference = difference;

            State = state;
        }
    }
}