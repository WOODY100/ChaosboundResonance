using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class ExpeditionInventoryPanelController : MonoBehaviour
    {
        [SerializeField]
        private GameObject inventoryPanel;

        private GameFlow gameFlow;

        private void OnEnable()
        {
            ResolveGameFlow();

            if (gameFlow == null)
            {
                return;
            }

            gameFlow.OnContextChanged += OnContextChanged;

            ApplyCurrentContext();
        }

        private void OnDisable()
        {
            if (gameFlow == null)
            {
                return;
            }

            gameFlow.OnContextChanged -= OnContextChanged;
            gameFlow = null;
        }

        private void ResolveGameFlow()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            gameFlow =
                bootstrapContext.GameFlow;
        }

        private void OnContextChanged(
            GameFlowContext previousContext,
            GameFlowContext currentContext)
        {
            ApplyContext(currentContext);
        }

        private void ApplyCurrentContext()
        {
            ApplyContext(gameFlow.CurrentContext);
        }

        private void ApplyContext(
            GameFlowContext context)
        {
            if (inventoryPanel == null)
            {
                return;
            }

            inventoryPanel.SetActive(
                context == GameFlowContext.Inventory);
        }
    }
}