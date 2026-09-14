using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chaosbound.Gameplay.Inventory.Testing
{
    public sealed class SecureInventoryDebugTest : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (!Keyboard.current.uKey.wasPressedThisFrame)
            {
                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null)
            {
                return;
            }

            if (persistentInventoryRuntime.State == null)
            {
                return;
            }

            SecureInventoryState secureInventory =
                persistentInventoryRuntime
                    .State
                    .SecureInventory;

            bool unlocked =
                secureInventory.TryUnlockNextSlot();

            if (unlocked)
            {
                UnityEngine.Debug.Log(
                    $"Secure slot unlocked. Total unlocked: " +
                    $"{secureInventory.UnlockedSlotCount}");
            }
            else
            {
                UnityEngine.Debug.Log(
                    "No more Secure Slots can be unlocked.");
            }
        }
    }
}