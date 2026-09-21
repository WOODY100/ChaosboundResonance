using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentRuntimeComposition : MonoBehaviour
    {
        [Header("Equipment Configuration")]
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private EquipmentProgressionConfig progressionConfig;

        private EquipmentStatsIntegration integration;
        private EquipmentProgressionRuntime progressionRuntime;

        public EquipmentProgressionRuntime ProgressionRuntime =>
            progressionRuntime;

        private void Start()
        {
            Compose();
        }

        private void OnDestroy()
        {
            DisposeIntegration();
        }

        private void Compose()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "BootstrapContext is not available.");

                return;
            }

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "BootstrapContext does not provide " +
                    "EquipmentLoadoutRuntime.");

                return;
            }

            PlayerModifierSystem playerModifierSystem =
                GetComponent<PlayerModifierSystem>();

            if (playerModifierSystem == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "PlayerModifierSystem was not found on this " +
                    "GameObject.");

                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "Missing EquipmentStatDatabase.");

                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "Missing EquipmentProgressionConfig.");

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "GameContentContext is not available.");

                return;
            }

            ItemContentResolver itemContentResolver =
                contentContext.ItemContentResolver;

            if (itemContentResolver == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "GameContentContext does not provide an " +
                    "ItemContentResolver.");

                return;
            }

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            EquipmentModifierSourceBuilder sourceBuilder =
                new EquipmentModifierSourceBuilder(
                    itemContentResolver,
                    statResolver);

            EquipmentTierOptionGenerator optionGenerator =
                new EquipmentTierOptionGenerator(
                    statDatabase,
                    progressionConfig.OptionsPerTier);

            progressionRuntime =
                new EquipmentProgressionRuntime(
                    progressionConfig,
                    optionGenerator);

            integration =
                new EquipmentStatsIntegration(
                    playerModifierSystem,
                    loadout,
                    sourceBuilder,
                    progressionRuntime);

            integration.Refresh();
        }

        private void DisposeIntegration()
        {
            if (integration == null)
                return;

            integration.Dispose();
            integration = null;
        }
    }
}