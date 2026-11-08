using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Equipment/Equipment Stat Definition",
        fileName = "EquipmentStatDefinition")]

    public sealed class EquipmentStatDefinition : ScriptableObject
    {
        [Header("Display Information")]

        [SerializeField]
        private string displayName;

        [TextArea(2, 4)]
        [SerializeField]
        private string description;

        [Header("Stat Configuration")]

        [SerializeField]
        private StatType statType;

        [SerializeField]
        private ModifierType modifierType;

        [SerializeField]
        private float minimumValue;

        [SerializeField]
        private float maximumValue;

        [SerializeField]
        private float upgradeGrowth;

        [SerializeField]
        private float weight = 1f;

        public string DisplayName =>
            displayName;

        public string Description =>
            description;

        public StatType StatType =>
            statType;

        public ModifierType ModifierType =>
            modifierType;

        public float MinimumValue =>
            minimumValue;

        public float MaximumValue =>
            maximumValue;

        public float UpgradeGrowth =>
            upgradeGrowth;

        public float Weight =>
            weight;

        private void OnValidate()
        {
            if (displayName != null)
                displayName = displayName.Trim();

            minimumValue = Mathf.Max(0f, minimumValue);

            maximumValue = Mathf.Max(
                minimumValue,
                maximumValue);

            upgradeGrowth = Mathf.Max(
                0f,
                upgradeGrowth);

            weight = Mathf.Max(
                0f,
                weight);
        }
    }
}