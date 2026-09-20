namespace Chaosbound.Gameplay.Equipment
{
    public readonly struct EquipmentComparisonStat
    {
        public StatType StatType { get; }

        public bool HasEquippedValue { get; }

        public bool HasCandidateValue { get; }

        public ModifierType EquippedModifierType { get; }

        public ModifierType CandidateModifierType { get; }

        public float EquippedValue { get; }

        public float CandidateValue { get; }

        public bool HasSameModifierType =>
            !HasEquippedValue ||
            !HasCandidateValue ||
            EquippedModifierType ==
            CandidateModifierType;

        public bool IsDirectlyComparable =>
            HasEquippedValue &&
            HasCandidateValue &&
            HasSameModifierType;

        public float Difference =>
            CandidateValue - EquippedValue;

        public EquipmentComparisonStat(
            StatType statType,
            bool hasEquippedValue,
            bool hasCandidateValue,
            ModifierType equippedModifierType,
            ModifierType candidateModifierType,
            float equippedValue,
            float candidateValue)
        {
            StatType = statType;

            HasEquippedValue =
                hasEquippedValue;

            HasCandidateValue =
                hasCandidateValue;

            EquippedModifierType =
                equippedModifierType;

            CandidateModifierType =
                candidateModifierType;

            EquippedValue =
                equippedValue;

            CandidateValue =
                candidateValue;
        }
    }
}