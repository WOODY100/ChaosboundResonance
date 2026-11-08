using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Currency.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithProgressionPanelController :
        MonoBehaviour
    {
        [Header("Selection")]
        [SerializeField]
        private BlacksmithItemDropTarget itemDropTarget;

        [Header("Selected Item")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image itemFrame;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemTier;
        [SerializeField] private TMP_Text upgradeLevel;
        [SerializeField] private GameObject emptySlotText;

        [Header("Progression")]
        [SerializeField]
        private EquipmentRuntimeComposition equipmentComposition;

        [SerializeField]
        private TierUpPreviewWindowController tierUpPreviewWindow;

        [SerializeField]
        private ItemTierPresentationDatabase tierPresentationDatabase;

        [SerializeField]
        private Image upgradeFill;

        [Header("Material Costs")]
        [SerializeField] private Transform materialCostContainer;
        [SerializeField] private MaterialCostEntryUI materialCostEntryPrefab;

        private readonly List<MaterialCostEntryUI> spawnedMaterialCostEntries =
            new List<MaterialCostEntryUI>();

        [Header("Reroll")]
        [SerializeField] private Button rerollButton;
        [SerializeField] private TMP_Text rerollCostText;

        [SerializeField] private Color rerollAffordableColor = Color.white;
        [SerializeField] private Color rerollUnaffordableColor = Color.red;

        private GenericConfirmationController confirmationController;

        private const string GoldContentId = "gold";

        [Header("Upgrade Action")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TMP_Text actionStatusText;

        [Header("Tab Mode")]
        [SerializeField]
        private BlacksmithProgressionTabController tabController;

        [SerializeField]
        private TMP_Text actionButtonText;

        private IReadOnlyList<MaterialCost> currentTierUpCosts;

        private bool IsTierUpActive =>
            tabController != null &&
            tabController.IsTierUpActive;

        [Header("Tier Up Options")]
        [SerializeField] private Transform tierUpOptionsContainer;
        [SerializeField] private BlacksmithTierOptionSlotUI tierOptionSlotPrefab;

        private readonly List<BlacksmithTierOptionSlotUI> spawnedTierOptions =
            new List<BlacksmithTierOptionSlotUI>();

        private EquipmentUpgradeSession TierUpSession =>
            equipmentComposition != null &&
            equipmentComposition.IsComposed
                ? equipmentComposition.UpgradeSession
                : null;

        // Cached cost for the next implementation block.
        private IReadOnlyList<MaterialCost> currentUpgradeCosts;

        private Sprite defaultItemIcon;

        private Color defaultItemFrameColor;
        private Color defaultItemTierColor;

        public ItemInstance SelectedItem =>
            itemDropTarget != null
                ? itemDropTarget.SelectedItem
                : null;

        public IReadOnlyList<MaterialCost> CurrentUpgradeCosts =>
            currentUpgradeCosts;

        private ItemInstance pendingRerollItem;

        private void Awake()
        {
            if (itemIcon != null)
                defaultItemIcon = itemIcon.sprite;

            if (itemFrame != null)
                defaultItemFrameColor = itemFrame.color;

            if (itemTier != null)
                defaultItemTierColor = itemTier.color;

            ValidateConfiguration();
        }

        private void OnEnable()
        {
            if (itemDropTarget != null)
            {
                itemDropTarget.SelectionChanged -=
                    HandleSelectionChanged;

                itemDropTarget.SelectionChanged +=
                    HandleSelectionChanged;
            }

            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveListener(
                    HandleUpgradeButtonClicked);

                upgradeButton.onClick.AddListener(
                    HandleUpgradeButtonClicked);
            }

            if (rerollButton != null)
            {
                rerollButton.onClick.RemoveListener(
                    HandleRerollButtonClicked);

                rerollButton.onClick.AddListener(
                    HandleRerollButtonClicked);
            }

            if (tabController != null)
            {
                tabController.ActiveTabChanged -=
                    HandleActiveTabChanged;

                tabController.ActiveTabChanged +=
                    HandleActiveTabChanged;
            }

            RefreshSelection();
        }

        private void OnDisable()
        {
            if (itemDropTarget != null)
            {
                itemDropTarget.SelectionChanged -=
                    HandleSelectionChanged;
            }

            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveListener(
                    HandleUpgradeButtonClicked);
            }

            if (rerollButton != null)
            {
                rerollButton.onClick.RemoveListener(
                    HandleRerollButtonClicked);
            }

            if (tabController != null)
            {
                tabController.ActiveTabChanged -=
                    HandleActiveTabChanged;
            }
        }

        //==================================================
        // Selection
        //==================================================

        private void HandleActiveTabChanged(bool isTierUpActive)
        {
            RefreshCurrentMode(SelectedItem);
        }

        private void HandleSelectionChanged(
            ItemInstance item,
            ItemBaseData itemData)
        {
            if (item == null || itemData == null)
            {
                ClearVisuals();
                return;
            }

            ShowSelectedItem(item, itemData);
        }

        private void RefreshSelection()
        {
            if (itemDropTarget == null)
                return;

            if (itemDropTarget.SelectedItem == null ||
                itemDropTarget.SelectedItemData == null)
            {
                ClearVisuals();
                return;
            }

            ShowSelectedItem(
                itemDropTarget.SelectedItem,
                itemDropTarget.SelectedItemData);
        }

        //==================================================
        // Selected Item Presentation
        //==================================================

        private void ShowSelectedItem(
            ItemInstance item,
            ItemBaseData itemData)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = itemData.Icon;
                itemIcon.enabled = itemData.Icon != null;
            }

            if (itemName != null)
            {
                itemName.text = itemData.DisplayName;
                itemName.gameObject.SetActive(true);
            }

            if (itemTier != null)
            {
                itemTier.text =
                    item.CurrentTier.ToString().ToUpperInvariant();

                itemTier.gameObject.SetActive(true);
            }

            ApplyTierPresentation(item.CurrentTier);

            if (upgradeLevel != null)
            {
                upgradeLevel.text =
                    $"Upgrade Level: {item.UpgradeLevel}";

                upgradeLevel.gameObject.SetActive(true);
            }

            if (itemName != null)
                itemName.gameObject.SetActive(true);

            if (emptySlotText != null)
                emptySlotText.SetActive(false);

            RefreshCurrentMode(item);
        }

        private void RefreshCurrentMode(ItemInstance item)
        {
            if (actionButtonText != null)
            {
                actionButtonText.text =
                    IsTierUpActive ? "TIER UP" : "UPGRADE";
            }

            if (item == null)
            {
                ClearMaterialCostEntries();
                
                ClearRerollCost();

                currentUpgradeCosts = null;
                currentTierUpCosts = null;

                if (upgradeButton != null)
                    upgradeButton.interactable = false;

                SetActionStatus("Selecciona un objeto.");
                return;
            }

            if (IsTierUpActive)
                RefreshTierUpProgress(item);
            else
            {
                ClearRerollCost();
                RefreshUpgradeProgress(item);
            }
        }

        private void ApplyTierPresentation(ItemTier tier)
        {
            if (tierPresentationDatabase == null)
            {
                UnityEngine.Debug.LogWarning(
                    "[BlacksmithProgressionPanelController] " +
                    "ItemTierPresentationDatabase is not assigned.",
                    this);

                return;
            }

            if (!tierPresentationDatabase.TryGet(
                    tier,
                    out ItemTierPresentationDefinition definition))
            {
                UnityEngine.Debug.LogWarning(
                    $"[BlacksmithProgressionPanelController] " +
                    $"No presentation found for tier: {tier}.",
                    this);

                return;
            }

            Color tierColor = definition.NameColor;

            if (itemTier != null)
                itemTier.color = tierColor;

            if (itemFrame != null)
                itemFrame.color = tierColor;
        }

        //==================================================
        // Upgrade Progress
        //==================================================

        private void RefreshUpgradeProgress(
            ItemInstance item)
        {
            if (upgradeButton != null)
                upgradeButton.interactable = false;

            currentUpgradeCosts = null;
            ClearMaterialCostEntries();

            if (item == null ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                ResetUpgradeProgress();

                UnityEngine.Debug.LogWarning(
                    "[BlacksmithProgressionPanelController] " +
                    "EquipmentRuntimeComposition is not ready.",
                    this);

                return;
            }

            EquipmentProgressionRuntime runtime =
                equipmentComposition.ProgressionRuntime;

            EquipmentUpgradeService upgradeService =
                equipmentComposition.UpgradeService;

            if (runtime == null || upgradeService == null)
            {
                ResetUpgradeProgress();
                return;
            }

            int upgradesPerTier =
                runtime.UpgradesPerTier;

            if (upgradesPerTier <= 0)
            {
                ResetUpgradeProgress();
                return;
            }

            int currentLevel =
                Mathf.Clamp(
                    item.UpgradeLevel,
                    0,
                    upgradesPerTier);

            if (upgradeFill != null)
            {
                upgradeFill.fillAmount =
                    (float)currentLevel / upgradesPerTier;
            }

            if (upgradeLevel != null)
            {
                upgradeLevel.text =
                    $"Upgrade Level: {currentLevel} / {upgradesPerTier}";
            }

            // Query the cost of the next normal upgrade.
            if (!runtime.CanUpgrade(item))
            {
                SetActionStatus("Nivel máximo alcanzado.");
                return;
            }

            if (!upgradeService.TryGetUpgradeCost(
                    item,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                SetActionStatus("Coste de mejora no disponible.");
                return;
            }

            currentUpgradeCosts = costs;

            RefreshMaterialCostEntries(
                upgradeService,
                currentUpgradeCosts);

            bool hasEnoughMaterials =
                HasEnoughUpgradeMaterials(
                    upgradeService,
                    currentUpgradeCosts);

            if (upgradeButton != null)
                upgradeButton.interactable = hasEnoughMaterials;

            SetActionStatus(
                hasEnoughMaterials
                    ? "Mejora disponible."
                    : "Materiales insuficientes.");
        }

        private void RefreshTierUpProgress(ItemInstance item)
        {
            currentTierUpCosts = null;
            currentUpgradeCosts = null;

            ClearRerollCost();

            ClearMaterialCostEntries();
            ClearTierOptionSlots();

            if (upgradeButton != null)
                upgradeButton.interactable = false;

            if (item == null ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                CloseTierUpSession();
                SetActionStatus("Sistema de progresión no disponible.");
                return;
            }

            EquipmentProgressionRuntime runtime =
                equipmentComposition.ProgressionRuntime;

            EquipmentUpgradeService upgradeService =
                equipmentComposition.UpgradeService;

            EquipmentUpgradeSession session = TierUpSession;

            if (runtime == null ||
                upgradeService == null ||
                session == null)
            {
                CloseTierUpSession();
                SetActionStatus("Sistema de progresión no disponible.");
                return;
            }

            if (item.CurrentTier == ItemTier.Legendary)
            {
                CloseTierUpSession();
                SetActionStatus("Tier máximo alcanzado.");
                return;
            }

            if (!runtime.IsReadyForTierUp(item))
            {
                CloseTierUpSession();

                SetActionStatus(
                    $"Necesitas Upgrade Level {runtime.UpgradesPerTier}/{runtime.UpgradesPerTier}.");

                return;
            }

            if (!upgradeService.TryGetTierUpCost(
                    item,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                CloseTierUpSession();
                SetActionStatus("Coste de Tier Up no disponible.");
                return;
            }

            currentTierUpCosts = costs;

            RefreshMaterialCostEntries(
                upgradeService,
                currentTierUpCosts);

            bool hasEnoughMaterials =
                HasEnoughUpgradeMaterials(
                    upgradeService,
                    currentTierUpCosts);

            // Reuse the existing session if it belongs to this item.
            if (!session.IsOpen ||
                !ReferenceEquals(session.ItemInstance, item))
            {
                session.Close();

                if (!session.Open(item))
                {
                    ClearTierOptionSlots();
                    SetActionStatus(
                        "No se pudieron generar las opciones.");

                    return;
                }
            }

            RefreshTierOptionSlots(session);

            RefreshRerollCost(
                item,
                upgradeService,
                session);

            if (session.HasSelection)
            {
                SetActionStatus(
                    hasEnoughMaterials
                        ? "Opción seleccionada."
                        : "Opción seleccionada. Materiales insuficientes.");
            }
            else
            {
                SetActionStatus(
                    hasEnoughMaterials
                        ? "Selecciona una opción."
                        : "Materiales insuficientes.");
            }

            bool canConfirmTierUp =
                session.IsOpen &&
                session.HasSelection &&
                hasEnoughMaterials;

            if (upgradeButton != null)
                upgradeButton.interactable = canConfirmTierUp;
        }

        private void ResetUpgradeProgress()
        {
            currentUpgradeCosts = null;
            currentTierUpCosts = null;

            if (upgradeFill != null)
                upgradeFill.fillAmount = 0f;

            if (upgradeLevel != null)
                upgradeLevel.text = string.Empty;
        }

        private void RefreshMaterialCostEntries(
            EquipmentUpgradeService upgradeService,
            IReadOnlyList<MaterialCost> costs)
        {
            ClearMaterialCostEntries();

            if (materialCostContainer == null ||
                materialCostEntryPrefab == null ||
                upgradeService == null ||
                costs == null)
            {
                return;
            }

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!upgradeService.TryGetMaterialDisplayData(
                        cost.ContentId,
                        out MaterialDefinition definition,
                        out int ownedAmount))
                {
                    UnityEngine.Debug.LogWarning(
                        "[BlacksmithProgressionPanelController] " +
                        $"Could not resolve material: {cost.ContentId}",
                        this);

                    continue;
                }

                MaterialCostEntryUI entry =
                    Instantiate(
                        materialCostEntryPrefab,
                        materialCostContainer);

                entry.SetData(
                    definition,
                    ownedAmount,
                    cost.Amount);

                spawnedMaterialCostEntries.Add(entry);
            }
        }

        private void ClearMaterialCostEntries()
        {
            for (int i = 0; i < spawnedMaterialCostEntries.Count; i++)
            {
                MaterialCostEntryUI entry =
                    spawnedMaterialCostEntries[i];

                if (entry != null)
                    Destroy(entry.gameObject);
            }

            spawnedMaterialCostEntries.Clear();
        }

        private bool HasEnoughUpgradeMaterials(
            EquipmentUpgradeService upgradeService,
            IReadOnlyList<MaterialCost> costs)
        {
            if (upgradeService == null ||
                costs == null ||
                costs.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!upgradeService.TryGetMaterialDisplayData(
                        cost.ContentId,
                        out _,
                        out int ownedAmount))
                {
                    return false;
                }

                if (ownedAmount < cost.Amount)
                    return false;
            }

            return true;
        }

        private void SetActionStatus(string message)
        {
            if (actionStatusText != null)
                actionStatusText.text = message;
        }

        private void HandleUpgradeButtonClicked()
        {
            if (IsTierUpActive)
            {
                HandleTierUpButtonClicked();
                return;
            }

            ItemInstance item = SelectedItem;

            if (item == null ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                return;
            }

            EquipmentUpgradeService upgradeService =
                equipmentComposition.UpgradeService;

            if (upgradeService == null)
                return;

            // Revalidate the current upgrade cost.
            if (!upgradeService.TryGetUpgradeCost(
                    item,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                RefreshUpgradeProgress(item);
                return;
            }

            // Prevent attempting an upgrade without enough materials.
            if (!HasEnoughUpgradeMaterials(
                    upgradeService,
                    costs))
            {
                RefreshUpgradeProgress(item);
                SetActionStatus("Materiales insuficientes.");
                return;
            }

            // Execute the normal upgrade.
            if (!upgradeService.TryUpgrade(item))
            {
                RefreshUpgradeProgress(item);
                SetActionStatus("No se pudo aplicar la mejora.");
                return;
            }

            // Refresh the selected item's progression.
            RefreshUpgradeProgress(item);

            SetActionStatus("¡Mejora aplicada correctamente!");
        }

        private void HandleTierUpButtonClicked()
        {
            ItemInstance item = SelectedItem;

            if (item == null ||
                itemDropTarget == null ||
                itemDropTarget.SelectedItemData == null ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                return;
            }

            EquipmentUpgradeSession session = TierUpSession;

            EquipmentUpgradeService upgradeService =
                equipmentComposition.UpgradeService;

            if (session == null ||
                upgradeService == null ||
                !session.IsOpen ||
                !session.HasSelection ||
                !ReferenceEquals(session.ItemInstance, item))
            {
                RefreshTierUpProgress(item);
                return;
            }

            // Revalidate Tier Up cost.
            if (!upgradeService.TryGetTierUpCost(
                    item,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                RefreshTierUpProgress(item);
                SetActionStatus("Coste de Tier Up no disponible.");
                return;
            }

            // Revalidate material availability.
            if (!HasEnoughUpgradeMaterials(
                    upgradeService,
                    costs))
            {
                RefreshTierUpProgress(item);
                SetActionStatus("Materiales insuficientes.");
                return;
            }

            if (tierUpPreviewWindow == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Tier Up Preview Window reference is missing.",
                    this);

                SetActionStatus(
                    "Ventana de confirmación no disponible.");

                return;
            }

            // Open the preview without modifying the item
            // or consuming materials.
            bool opened = tierUpPreviewWindow.OpenPreview(
                item,
                itemDropTarget.SelectedItemData);

            if (!opened)
            {
                SetActionStatus(
                    "No se pudo generar la vista previa.");

                return;
            }

            SetActionStatus("Revisa los cambios antes de confirmar.");
        }

        public void RefreshAfterTierUp(ItemInstance item)
        {
            if (item == null ||
                itemDropTarget == null ||
                !ReferenceEquals(itemDropTarget.SelectedItem, item))
            {
                return;
            }

            ItemBaseData itemData =
                itemDropTarget.SelectedItemData;

            if (itemData == null)
                return;

            // Refresh selected equipment presentation.
            ShowSelectedItem(item, itemData);

            // Refresh progression, costs and available options.
            RefreshTierUpProgress(item);

            SetActionStatus("¡Tier Up aplicado correctamente!");
        }

        private void RefreshTierOptionSlots(
            EquipmentUpgradeSession session)
        {
            ClearTierOptionSlots();

            if (session == null ||
                !session.IsOpen ||
                tierUpOptionsContainer == null ||
                tierOptionSlotPrefab == null)
            {
                return;
            }

            EquipmentStatDatabase statDatabase =
                equipmentComposition != null
                    ? equipmentComposition.StatDatabase
                    : null;

            if (statDatabase == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "EquipmentStatDatabase is not available.",
                    this);

                return;
            }

            IReadOnlyList<EquipmentStatOption> options =
                session.Options;

            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption option = options[i];

                if (!statDatabase.TryGetDefinition(
                        option.StatType,
                        out EquipmentStatDefinition definition))
                {
                    UnityEngine.Debug.LogError(
                        "[BlacksmithProgressionPanelController] " +
                        $"Missing EquipmentStatDefinition for '{option.StatType}'.",
                        this);

                    continue;
                }

                BlacksmithTierOptionSlotUI slot =
                    Instantiate(
                        tierOptionSlotPrefab,
                        tierUpOptionsContainer);

                slot.SetData(
                    definition.DisplayName,
                    FormatStatValue(option),
                    definition.Description);

                bool isSelected =
                    session.HasSelection &&
                    IsSameOption(
                        session.SelectedOption,
                        option);

                slot.SetSelected(isSelected);

                EquipmentStatOption capturedOption = option;

                if (slot.SelectionButton != null)
                {
                    slot.SelectionButton.onClick.AddListener(
                        () => HandleTierOptionSelected(capturedOption));
                }

                spawnedTierOptions.Add(slot);
            }
        }

        private void HandleTierOptionSelected(
            EquipmentStatOption option)
        {
            EquipmentUpgradeSession session = TierUpSession;

            if (session == null || !session.IsOpen)
                return;

            if (!session.TrySelect(option))
                return;

            for (int i = 0; i < spawnedTierOptions.Count; i++)
            {
                BlacksmithTierOptionSlotUI slot =
                    spawnedTierOptions[i];

                if (slot == null)
                    continue;

                slot.SetSelected(false);
            }

            for (int i = 0; i < spawnedTierOptions.Count; i++)
            {
                BlacksmithTierOptionSlotUI slot =
                    spawnedTierOptions[i];

                if (slot == null)
                    continue;

                // The selected option is identified by its displayed data.
                // The session remains the authority for selection.
                slot.SetSelected(
                    IsSameOption(
                        session.SelectedOption,
                        GetOptionForSlot(i, session.Options)));
            }

            SetActionStatus("Opción seleccionada.");

            bool hasEnoughMaterials =
                equipmentComposition != null &&
                equipmentComposition.IsComposed &&
                HasEnoughUpgradeMaterials(
                    equipmentComposition.UpgradeService,
                    currentTierUpCosts);

            if (upgradeButton != null)
            {
                upgradeButton.interactable =
                    session.HasSelection &&
                    hasEnoughMaterials;
            }

            if (!hasEnoughMaterials)
            {
                SetActionStatus(
                    "Opción seleccionada. Materiales insuficientes.");
            }
        }

        private EquipmentStatOption GetOptionForSlot(
            int index,
            IReadOnlyList<EquipmentStatOption> options)
        {
            if (options == null ||
                index < 0 ||
                index >= options.Count)
            {
                return default;
            }

            return options[index];
        }

        private void HandleRerollButtonClicked()
        {
            ItemInstance item = SelectedItem;

            EquipmentUpgradeSession session = TierUpSession;

            if (item == null ||
                session == null ||
                !session.IsOpen ||
                !ReferenceEquals(session.ItemInstance, item) ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                return;
            }

            EquipmentUpgradeService upgradeService =
                equipmentComposition.UpgradeService;

            if (upgradeService == null ||
                !upgradeService.TryGetRerollCost(
                    item,
                    out int goldCost))
            {
                SetActionStatus("Coste de Reroll no disponible.");
                return;
            }

            if (!ResolveConfirmationController())
            {
                SetActionStatus("Confirmación no disponible.");
                return;
            }

            BootstrapContext context = BootstrapContext.Current;

            PersistentCurrencyState currencyState =
                context != null
                    ? context.PersistentCurrencyState
                    : null;

            if (currencyState == null)
                return;

            int currentGold = currencyState.GetAmount(GoldContentId);

            if (currentGold < goldCost)
            {
                RefreshRerollCost(item, upgradeService, session);
                SetActionStatus("Gold insuficiente para realizar el Reroll.");
                return;
            }

            pendingRerollItem = item;

            confirmationController.Show(
                "Confirmar Reroll",
                $"¿Deseas generar tres nuevas opciones de estadísticas por {goldCost} Gold?",
                "CANCELAR",
                "CONFIRMAR",
                HandleRerollConfirmed,
                HandleRerollCancelled);
        }

        private void HandleRerollConfirmed()
        {
            ItemInstance item = pendingRerollItem;
            pendingRerollItem = null;

            if (item == null ||
                !ReferenceEquals(SelectedItem, item) ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                return;
            }

            EquipmentUpgradeSession session = TierUpSession;

            if (session == null ||
                !session.IsOpen ||
                !ReferenceEquals(session.ItemInstance, item))
            {
                return;
            }

            if (!session.TryReroll())
            {
                RefreshTierUpProgress(item);
                SetActionStatus(
                    "No se pudo realizar el Reroll. Verifica tu Gold.");

                return;
            }

            RefreshTierUpProgress(item);

            SetActionStatus("¡Nuevas opciones generadas correctamente!");
        }

        private void HandleRerollCancelled()
        {
            pendingRerollItem = null;
        }

        private void ClearTierOptionSlots()
        {
            for (int i = 0; i < spawnedTierOptions.Count; i++)
            {
                BlacksmithTierOptionSlotUI slot =
                    spawnedTierOptions[i];

                if (slot != null)
                    Destroy(slot.gameObject);
            }

            spawnedTierOptions.Clear();
        }

        private void CloseTierUpSession()
        {
            EquipmentUpgradeSession session = TierUpSession;

            if (session != null)
                session.Close();

            ClearTierOptionSlots();
        }

        private static bool IsSameOption(
            EquipmentStatOption first,
            EquipmentStatOption second)
        {
            return first.StatType == second.StatType &&
                   first.ModifierType == second.ModifierType &&
                   Mathf.Approximately(
                       first.RolledValue,
                       second.RolledValue);
        }

        private string FormatStatValue(EquipmentStatOption option)
        {
            switch (option.ModifierType)
            {
                case ModifierType.Flat:
                    return $"+{option.RolledValue:0.##}";

                case ModifierType.Percent:
                    return $"+{option.RolledValue:0.##}%";

                case ModifierType.FinalMultiplier:
                    return $"{option.RolledValue:0.##}x";

                default:
                    return option.RolledValue.ToString("0.##");
            }
        }

        private bool ResolveConfirmationController()
        {
            BootstrapContext context = BootstrapContext.Current;

            confirmationController = context != null
                ? context.GetComponentInChildren
                    <GenericConfirmationController>(true)
                : null;

            if (confirmationController == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "GenericConfirmationController was not found in Bootstrap.",
                    this);

                return false;
            }

            return true;
        }

        private void RefreshRerollCost(
            ItemInstance item,
            EquipmentUpgradeService upgradeService,
            EquipmentUpgradeSession session)
        {
            if (rerollButton == null || rerollCostText == null)
                return;

            if (item == null ||
                session == null ||
                !session.IsOpen ||
                !ReferenceEquals(session.ItemInstance, item) ||
                upgradeService == null)
            {
                ClearRerollCost();
                return;
            }

            if (!upgradeService.TryGetRerollCost(
                    item,
                    out int goldCost))
            {
                ClearRerollCost();
                return;
            }

            BootstrapContext context = BootstrapContext.Current;

            PersistentCurrencyState currencyState =
                context != null
                    ? context.PersistentCurrencyState
                    : null;

            if (currencyState == null)
            {
                rerollCostText.text = goldCost.ToString();
                rerollCostText.color = rerollUnaffordableColor;
                rerollButton.interactable = false;
                return;
            }

            int currentGold = currencyState.GetAmount(GoldContentId);

            bool canAfford = currentGold >= goldCost;

            rerollCostText.text = goldCost.ToString();
            rerollCostText.color = canAfford
                ? rerollAffordableColor
                : rerollUnaffordableColor;

            rerollButton.interactable = true;
        }

        //==================================================
        // Clear Selection
        //==================================================

        private void ClearRerollCost()
        {
            if (rerollCostText != null)
                rerollCostText.text = string.Empty;

            if (rerollButton != null)
                rerollButton.interactable = false;
        }

        private void ClearVisuals()
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = defaultItemIcon;
                itemIcon.enabled = defaultItemIcon != null;
            }

            if (itemName != null)
            {
                itemName.text = string.Empty;
                itemName.gameObject.SetActive(false);
            }

            if (itemTier != null)
            {
                itemTier.text = string.Empty;
                itemTier.gameObject.SetActive(false);
            }

            if (upgradeLevel != null)
            {
                upgradeLevel.text = string.Empty;
                upgradeLevel.gameObject.SetActive(false);
            }

            currentUpgradeCosts = null;
            currentTierUpCosts = null;
            CloseTierUpSession();

            if (itemTier != null)
                itemTier.color = defaultItemTierColor;

            if (itemFrame != null)
                itemFrame.color = defaultItemFrameColor;

            if (actionButtonText != null)
            {
                actionButtonText.text =
                    IsTierUpActive ? "TIER UP" : "UPGRADE";
            }

            if (upgradeButton != null)
                upgradeButton.interactable = false;

            SetActionStatus("Selecciona un objeto.");

            if (emptySlotText != null)
                emptySlotText.SetActive(true);

            ClearRerollCost();
            ClearMaterialCostEntries();
            ResetUpgradeProgress();
        }

        //==================================================
        // Validation
        //==================================================

        private void ValidateConfiguration()
        {
            if (tabController == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Tab Controller reference is missing.",
                    this);
            }

            if (actionButtonText == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Action Button Text reference is missing.",
                    this);
            }

            if (itemDropTarget == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Item Drop Target reference is missing.",
                    this);
            }

            if (itemIcon == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Item Icon reference is missing.",
                    this);
            }

            if (itemName == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Item Name reference is missing.",
                    this);
            }

            if (equipmentComposition == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Equipment Runtime Composition reference is missing.",
                    this);
            }

            if (upgradeFill == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Upgrade Fill reference is missing.",
                    this);
            }

            if (materialCostContainer == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Material Cost Container reference is missing.",
                    this);
            }

            if (materialCostEntryPrefab == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Material Cost Entry Prefab reference is missing.",
                    this);
            }

            if (upgradeButton == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Upgrade Button reference is missing.",
                    this);
            }

            if (actionStatusText == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Action Status Text reference is missing.",
                    this);
            }

            if (tierUpOptionsContainer == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Tier Up Options Container reference is missing.",
                    this);
            }

            if (tierOptionSlotPrefab == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Tier Option Slot Prefab reference is missing.",
                    this);
            }

            if (itemFrame == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Item Frame reference is missing.",
                    this);
            }

            if (tierPresentationDatabase == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionPanelController] " +
                    "Item Tier Presentation Database reference is missing.",
                    this);
            }
        }
    }
}