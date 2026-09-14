using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class ExpeditionInventoryInput : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private InputActionReference inventoryAction;

        private void OnEnable()
        {
            if (inventoryAction == null)
            {
                return;
            }

            inventoryAction.action.Enable();
            inventoryAction.action.performed += OnInventoryPerformed;
        }

        private void OnDisable()
        {
            if (inventoryAction == null)
            {
                return;
            }

            inventoryAction.action.performed -= OnInventoryPerformed;
            inventoryAction.action.Disable();
        }

        private void OnInventoryPerformed(
            InputAction.CallbackContext context)
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            GameFlow gameFlow =
                bootstrapContext.GameFlow;

            if (gameFlow == null ||
                !gameFlow.IsInitialized)
            {
                return;
            }

            if (gameFlow.CurrentContext ==
                GameFlowContext.Inventory)
            {
                gameFlow.Pop(
                    GameFlowContext.Inventory);

                return;
            }

            gameFlow.Request(
                GameFlowContext.Inventory);
        }
    }
}