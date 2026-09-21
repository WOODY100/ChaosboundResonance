using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerModifierSystem modifierSystem;
        [SerializeField] private CharacterStatsPanelUI panelUI;

        private CharacterStatsDisplayBuilder displayBuilder;

        private void Awake()
        {
            if (modifierSystem == null)
            {
                Debug.LogError(
                    "[CharacterStatsPanelController] " +
                    "PlayerModifierSystem is not assigned.",
                    this);

                return;
            }

            if (panelUI == null)
            {
                Debug.LogError(
                    "[CharacterStatsPanelController] " +
                    "CharacterStatsPanelUI is not assigned.",
                    this);

                return;
            }

            displayBuilder =
                new CharacterStatsDisplayBuilder(
                    modifierSystem);
        }

        private void OnEnable()
        {
            if (modifierSystem == null)
                return;

            modifierSystem.OnStatChanged += OnStatChanged;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDisable()
        {
            if (modifierSystem == null)
                return;

            modifierSystem.OnStatChanged -= OnStatChanged;
        }

        private void OnStatChanged(
            StatType statType,
            float value)
        {
            Refresh();
        }

        public void Refresh()
        {
            if (displayBuilder == null)
                return;

            if (panelUI == null)
                return;

            CharacterStatsDisplayData displayData =
                displayBuilder.Build();

            panelUI.Show(displayData);
        }
    }
}