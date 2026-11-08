using Chaosbound.Core.Composition;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentRuntimeComposition : MonoBehaviour
    {
        [Header("Equipment Configuration")]

        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [SerializeField]
        private EquipmentProgressionConfig progressionConfig;

        private EquipmentStatsIntegration integration;
        private EquipmentProgressionRuntime progressionRuntime;
        private EquipmentUpgradeService upgradeService;
        private EquipmentUpgradeSession upgradeSession;

        private bool isComposed;

        //==========================================================
        // Public Runtime Access
        //==========================================================

        public EquipmentProgressionRuntime ProgressionRuntime =>
            progressionRuntime;

        public EquipmentUpgradeService UpgradeService =>
            upgradeService;

        public EquipmentUpgradeSession UpgradeSession =>
            upgradeSession;

        public bool IsComposed =>
            isComposed;

        public EquipmentStatDatabase StatDatabase =>
            statDatabase;

        //==========================================================
        // Unity Lifecycle
        //==========================================================

        private void Start()
        {
            Compose();
        }

        private void OnDestroy()
        {
            DisposeComposition();
        }

        //==========================================================
        // Composition
        //==========================================================

        private void Compose()
        {
            if (isComposed)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "BootstrapContext is not available.",
                    this);

                return;
            }

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "EquipmentLoadoutRuntime is not available.",
                    this);

                return;
            }

            PersistentInventoryRuntime inventoryRuntime =
                bootstrapContext.PersistentInventoryRuntime;

            if (inventoryRuntime == null ||
                inventoryRuntime.State == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "PersistentInventoryRuntime is not ready.",
                    this);

                return;
            }

            PersistentMaterialsState materialsState =
                inventoryRuntime.State.Materials;

            if (materialsState == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "PersistentMaterialsState is not available.",
                    this);

                return;
            }

            PlayerModifierSystem playerModifierSystem =
                GetComponent<PlayerModifierSystem>();

            if (playerModifierSystem == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "PlayerModifierSystem was not found on this " +
                    "GameObject.",
                    this);

                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "Missing EquipmentStatDatabase.",
                    this);

                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "Missing EquipmentProgressionConfig.",
                    this);

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "GameContentContext is not available.",
                    this);

                return;
            }

            ItemContentResolver itemContentResolver =
                contentContext.ItemContentResolver;

            if (itemContentResolver == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "ItemContentResolver is not available.",
                    this);

                return;
            }

            MaterialResolver materialResolver =
                contentContext.MaterialResolver;

            if (materialResolver == null)
            {
                Debug.LogError(
                    "[Equipment Runtime Composition] " +
                    "MaterialResolver is not available.",
                    this);

                return;
            }

            //======================================================
            // Equipment Stat Infrastructure
            //======================================================

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

            //======================================================
            // Progression Runtime
            //======================================================

            progressionRuntime =
                new EquipmentProgressionRuntime(
                    progressionConfig,
                    optionGenerator);

            //======================================================
            // Upgrade Service
            //======================================================

            upgradeService =
                new EquipmentUpgradeService(
                    itemContentResolver,
                    materialResolver,
                    materialsState,
                    progressionConfig,
                    progressionRuntime,
                    bootstrapContext,
                    bootstrapContext.PersistentCurrencyState);

            //======================================================
            // Upgrade Session
            //======================================================

            upgradeSession =
                new EquipmentUpgradeSession(
                    upgradeService,
                    bootstrapContext);

            //======================================================
            // Equipment Stats Integration
            //======================================================

            integration =
                new EquipmentStatsIntegration(
                    playerModifierSystem,
                    loadout,
                    sourceBuilder,
                    progressionRuntime);

            integration.Refresh();

            isComposed = true;
        }

        //==========================================================
        // Disposal
        //==========================================================

        private void DisposeComposition()
        {
            if (integration != null)
            {
                integration.Dispose();
                integration = null;
            }

            if (upgradeSession != null)
            {
                upgradeSession.Close();
                upgradeSession = null;
            }

            upgradeService = null;
            progressionRuntime = null;

            isComposed = false;
        }
    }
}