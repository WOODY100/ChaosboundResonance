using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class TierUpPreviewWindowController : MonoBehaviour
    {
        [Header("Window")]
        [SerializeField] private GameObject confirmationWindow;
        [SerializeField] private TMP_Text title;

        [Header("Dependencies")]
        [SerializeField] private EquipmentRuntimeComposition equipmentComposition;
        [SerializeField] private BlacksmithProgressionPanelController progressionPanel;

        [SerializeField]
        private ItemTierPresentationDatabase tierPresentationDatabase;

        [Header("Current Equipment")]
        [SerializeField] private Image currentIcon;
        [SerializeField] private Image currentFrame;
        [SerializeField] private TMP_Text currentTierText;
        [SerializeField] private TMP_Text currentUpgradeLevelText;
        [SerializeField] private Transform currentStatsContent;

        [Header("Projected Equipment")]
        [SerializeField] private Image projectedIcon;
        [SerializeField] private Image projectedFrame;
        [SerializeField] private TMP_Text projectedTierText;
        [SerializeField] private TMP_Text projectedUpgradeLevelText;
        [SerializeField] private Transform projectedStatsContent;

        [Header("Stat Transfer")]
        [SerializeField] private TMP_Text transferStatText;
        [SerializeField] private TMP_Text transferValueText;

        [Header("Material Costs")]
        [SerializeField] private Transform materialContent;
        [SerializeField] private MaterialCostEntryUI materialCostEntryPrefab;

        [Header("Stat Prefab")]
        [SerializeField] private EquipmentStatRowUI statRowPrefab;

        [Header("Actions")]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;

        private readonly List<GameObject> currentStatRows =
            new List<GameObject>();

        private readonly List<GameObject> projectedStatRows =
            new List<GameObject>();

        private readonly List<GameObject> materialEntries =
            new List<GameObject>();

        private ItemInstance previewedItem;

        private void Awake()
        {
            if (confirmationWindow != null)
                confirmationWindow.SetActive(false);
        }

        private void OnEnable()
        {
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(Cancel);
                cancelButton.onClick.AddListener(Cancel);
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(Confirm);
                confirmButton.onClick.AddListener(Confirm);
            }
        }

        private void OnDisable()
        {
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(Cancel);

            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(Confirm);
        }

        public bool OpenPreview(
            ItemInstance item,
            ItemBaseData baseData)
        {
            if (item == null)
            {
                UnityEngine.Debug.LogError("[TierUpPreview] Item is null.", this);
                return false;
            }

            if (baseData == null)
            {
                UnityEngine.Debug.LogError("[TierUpPreview] ItemBaseData is null.", this);
                return false;
            }

            if (equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                UnityEngine.Debug.LogError(
                    "[TierUpPreview] EquipmentComposition is unavailable.",
                    this);
                return false;
            }

            EquipmentUpgradeSession session =
                equipmentComposition.UpgradeSession;

            if (session == null)
            {
                UnityEngine.Debug.LogError("[TierUpPreview] Session is null.", this);
                return false;
            }

            if (!session.IsOpen)
            {
                UnityEngine.Debug.LogError("[TierUpPreview] Session is not open.", this);
                return false;
            }

            if (!session.HasSelection)
            {
                UnityEngine.Debug.LogError("[TierUpPreview] Session has no selection.", this);
                return false;
            }

            if (!ReferenceEquals(session.ItemInstance, item))
            {
                UnityEngine.Debug.LogError(
                    "[TierUpPreview] Session item does not match selected item.",
                    this);
                return false;
            }

            TierUpPreviewDataBuilder builder =
                new TierUpPreviewDataBuilder(
                    equipmentComposition.StatDatabase,
                    equipmentComposition.UpgradeService);

            if (!builder.TryBuild(
                    item,
                    baseData,
                    session.SelectedOption,
                    out TierUpPreviewData previewData))
            {
                UnityEngine.Debug.LogError(
                    "[TierUpPreview] TierUpPreviewDataBuilder.TryBuild returned false.",
                    this);
                return false;
            }

            UnityEngine.Debug.Log("[TierUpPreview] Data successfully generated.", this);

            if (!PopulateWindow(previewData))
            {
                UnityEngine.Debug.LogError(
                    "[TierUpPreview] PopulateWindow returned false. " +
                    "Check stats and material containers/prefabs.",
                    this);
                return false;
            }

            previewedItem = item;
            confirmationWindow.SetActive(true);

            UnityEngine.Debug.Log("[TierUpPreview] Window opened successfully.", this);

            return true;
        }

        private bool PopulateWindow(TierUpPreviewData data)
        {
            if (data == null)
                return false;

            ClearGeneratedEntries();

            if (title != null)
                title.text = $"Confirmar Tier Up - {data.ItemName}";

            SetEquipmentPresentation(
                currentIcon,
                currentFrame,
                currentTierText,
                currentUpgradeLevelText,
                data.ItemIcon,
                data.CurrentTier,
                data.CurrentUpgradeLevel);

            SetEquipmentPresentation(
                projectedIcon,
                projectedFrame,
                projectedTierText,
                projectedUpgradeLevelText,
                data.ItemIcon,
                data.ProjectedTier,
                data.ProjectedUpgradeLevel);

            if (!PopulateStats(
                    data.CurrentStats,
                    currentStatsContent,
                    currentStatRows))
            {
                return false;
            }

            if (!PopulateStats(
                    data.ProjectedStats,
                    projectedStatsContent,
                    projectedStatRows))
            {
                return false;
            }

            if (transferStatText != null)
                transferStatText.text = data.NewStatName;

            if (transferValueText != null)
                transferValueText.text = data.NewStatValue;

            return PopulateMaterialCosts(data.MaterialCosts);
        }

        private void SetEquipmentPresentation(
            Image icon,
            Image frame,
            TMP_Text tierText,
            TMP_Text levelText,
            Sprite itemSprite,
            ItemTier tier,
            int upgradeLevel)
        {
            if (icon != null)
            {
                icon.sprite = itemSprite;
                icon.enabled = itemSprite != null;
            }

            Color tierColor = Color.white;

            if (tierPresentationDatabase != null &&
                tierPresentationDatabase.TryGet(
                    tier,
                    out ItemTierPresentationDefinition definition))
            {
                tierColor = definition.NameColor;
            }

            if (frame != null)
                frame.color = tierColor;

            if (tierText != null)
            {
                tierText.text = tier.ToString();
                tierText.color = tierColor;
            }

            if (levelText != null)
                levelText.text = $"Upgrade Lv. {upgradeLevel}";
        }

        private bool PopulateStats(
            IReadOnlyList<EquipmentStatPreviewEntry> stats,
            Transform container,
            List<GameObject> spawnedRows)
        {
            if (container == null || statRowPrefab == null ||
                stats == null)
            {
                return false;
            }

            for (int i = 0; i < stats.Count; i++)
            {
                EquipmentStatPreviewEntry entry = stats[i];

                if (entry == null)
                    return false;

                EquipmentStatRowUI row =
                    Instantiate(statRowPrefab, container);

                if (row == null)
                    return false;

                row.SetData(
                    entry.DisplayName,
                    entry.DisplayValue,
                    entry.IsNew);

                spawnedRows.Add(row.gameObject);
            }

            return true;
        }

        private bool PopulateMaterialCosts(
            IReadOnlyList<MaterialCostPreviewEntry> costs)
        {
            if (materialContent == null ||
                materialCostEntryPrefab == null ||
                costs == null)
            {
                return false;
            }

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCostPreviewEntry cost = costs[i];

                if (cost == null || cost.Definition == null)
                    return false;

                MaterialCostEntryUI entry =
                    Instantiate(
                        materialCostEntryPrefab,
                        materialContent);

                if (entry == null)
                    return false;

                entry.SetData(
                    cost.Definition,
                    cost.OwnedAmount,
                    cost.RequiredAmount);

                materialEntries.Add(entry.gameObject);
            }

            return true;
        }

        private void Confirm()
        {
            if (previewedItem == null ||
                equipmentComposition == null ||
                !equipmentComposition.IsComposed)
            {
                return;
            }

            EquipmentUpgradeSession session =
                equipmentComposition.UpgradeSession;

            if (session == null ||
                !session.IsOpen ||
                !session.HasSelection ||
                !ReferenceEquals(session.ItemInstance, previewedItem))
            {
                return;
            }

            if (!session.TryConfirm())
                return;

            ItemInstance confirmedItem = previewedItem;

            previewedItem = null;
            confirmationWindow.SetActive(false);

            if (progressionPanel != null)
                progressionPanel.RefreshAfterTierUp(confirmedItem);
        }

        private void Cancel()
        {
            previewedItem = null;

            if (confirmationWindow != null)
                confirmationWindow.SetActive(false);
        }

        private void ClearGeneratedEntries()
        {
            ClearEntries(currentStatRows);
            ClearEntries(projectedStatRows);
            ClearEntries(materialEntries);
        }

        private static void ClearEntries(List<GameObject> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null)
                    Destroy(entries[i]);
            }

            entries.Clear();
        }
    }
}