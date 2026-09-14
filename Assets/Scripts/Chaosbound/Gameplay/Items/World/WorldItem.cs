using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Presentation;
using UnityEngine;


namespace Chaosbound.Gameplay.Items.World
{
    public sealed class WorldItem : PooledBehaviour, IInteractable
    {
        [SerializeField]
        private WorldItemPresentation presentation;

        private ItemInstance itemInstance;
        private ItemBaseData baseData;

        public ItemInstance ItemInstance =>
            itemInstance;

        public ItemBaseData BaseData =>
            baseData;

        public bool IsInitialized =>
            itemInstance != null && baseData != null;

        public void Initialize(
            ItemInstance instance,
            ItemBaseData definition)
        {
            if (instance == null)
            {
                return;
            }

            if (definition == null)
            {
                return;
            }

            if (instance.BaseDataId != definition.ContentId)
            {
                return;
            }

            itemInstance = instance;
            baseData = definition;

            presentation?.Apply(
                baseData,
                itemInstance);
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (interactor == null)
            {
                return;
            }

            TryCollect();
        }

        private bool TryCollect()
        {
            if (!IsInitialized)
            {
                return false;
            }

            RunManager runManager =
                RunManager.Instance;

            if (runManager == null)
            {
                return false;
            }

            if (runManager.ExpeditionRuntimeState == null)
            {
                return false;
            }

            ExpeditionInventoryRuntime inventory =
                runManager.ExpeditionRuntimeState.Inventory;

            if (inventory == null)
            {
                return false;
            }

            if (!inventory.TryAdd(itemInstance))
            {
                return false;
            }

            ReturnToPool();

            return true;
        }

        protected override void ResetPooledState()
        {
            itemInstance = null;
            baseData = null;

            presentation?.Clear();
        }
    }
}