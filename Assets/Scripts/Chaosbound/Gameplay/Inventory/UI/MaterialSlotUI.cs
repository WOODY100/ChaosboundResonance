using Chaosbound.Content.Materials;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class MaterialSlotUI :
        MonoBehaviour,
        ITooltipSource,
        ITooltipSeenSource
    {
        [Header("References")]
        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private TMP_Text amountText;

        [Header("New Indicator")]
        [SerializeField]
        private GameObject newIndicator;

        private string materialId;

        private MaterialResolver resolver;

        public string MaterialId =>
            materialId;

        private void Awake()
        {
            ResolveMaterialDatabase();
        }

        private void ResolveMaterialDatabase()
        {
            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
                return;

            if (contentContext.MaterialDatabase == null)
                return;

            resolver =
                new MaterialResolver(
                    contentContext.MaterialDatabase);
        }

        public TooltipContent GetTooltipContent()
        {
            if (string.IsNullOrEmpty(materialId))
                return null;

            if (resolver == null)
            {
                ResolveMaterialDatabase();

                if (resolver == null)
                    return null;
            }

            if (!resolver.TryResolve(
                    materialId,
                    out MaterialDefinition definition))
            {
                return null;
            }

            int amount = 0;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext != null)
            {
                if (bootstrapContext.PersistentInventoryRuntime != null)
                {
                    amount =
                        bootstrapContext
                            .PersistentInventoryRuntime
                            .State
                            .Materials
                            .GetAmount(materialId);
                }
            }

            return TooltipContentFactory.CreateMaterialContent(
                definition,
                amount);
        }

        public void MarkAsSeen()
        {
            if (string.IsNullOrEmpty(materialId))
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            PersistentInventoryRuntime inventoryRuntime =
                bootstrapContext.PersistentInventoryRuntime;

            if (inventoryRuntime == null)
                return;

            PersistentInventoryState state =
                inventoryRuntime.State;

            if (state == null)
                return;

            state.MaterialSeenState.MarkSeen(
                materialId);

            SetNewIndicator(false);
        }

        public void SetMaterial(
            string materialId,
            Sprite icon,
            int amount)
        {
            if (string.IsNullOrEmpty(materialId))
            {
                Clear();
                return;
            }

            this.materialId = materialId;

            if (itemIcon != null)
            {
                itemIcon.sprite = icon;
                itemIcon.enabled = icon != null;
            }

            if (amountText != null)
            {
                amountText.text = amount.ToString();
            }
        }

        public void SetNewIndicator(bool isNew)
        {
            if (newIndicator == null)
                return;

            newIndicator.SetActive(isNew);
        }

        public void Clear()
        {
            materialId = null;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (amountText != null)
            {
                amountText.text = string.Empty;
            }

            SetNewIndicator(false);
        }
    }
}