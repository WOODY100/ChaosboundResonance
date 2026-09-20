using System;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentInventoryService
    {
        private readonly PersistentInventoryRuntime inventory;
        private readonly ItemContentResolver contentResolver;
        private readonly EquipmentLoadoutRuntime loadout;

        public EquipmentInventoryService(
            PersistentInventoryRuntime inventory,
            ItemContentResolver contentResolver,
            EquipmentLoadoutRuntime loadout)
        {
            this.inventory =
                inventory ??
                throw new ArgumentNullException(
                    nameof(inventory));

            this.contentResolver =
                contentResolver ??
                throw new ArgumentNullException(
                    nameof(contentResolver));

            this.loadout =
                loadout ??
                throw new ArgumentNullException(
                    nameof(loadout));
        }

        public bool TryEquip(
            ItemInstance itemInstance,
            out ItemInstance replacedItem)
        {
            replacedItem = null;

            if (itemInstance == null)
                return false;

            if (!contentResolver.TryResolve(
                itemInstance.BaseDataId,
                out ItemBaseData baseData))
            {
                return false;
            }

            if (baseData == null)
                return false;

            if (baseData.Category !=
                ItemCategory.Equipment)
            {
                return false;
            }

            if (baseData.EquipmentType ==
                EquipmentType.None)
            {
                return false;
            }

            if (!inventory.State.Items.Contains(
                    itemInstance))
            {
                return false;
            }

            if (loadout.IsEquipped(
                    itemInstance))
            {
                return false;
            }

            return loadout.TryEquip(
                itemInstance,
                baseData,
                out replacedItem);
        }

        public bool TryUnequip(
            EquipmentType equipmentType,
            out ItemInstance removedItem)
        {
            removedItem = null;

            if (equipmentType ==
                EquipmentType.None)
            {
                return false;
            }

            return loadout.TryUnequip(
                equipmentType,
                out removedItem);
        }

        public bool TryUnequipToInventory(
            EquipmentType equipmentType,
            int? targetSlotIndex,
            out ItemInstance removedItem,
            out ItemInstance replacedItem)
        {
            removedItem = null;
            replacedItem = null;

            if (equipmentType ==
                EquipmentType.None)
            {
                return false;
            }

            if (!loadout.TryGetEquipped(
                    equipmentType,
                    out ItemInstance equippedItem))
            {
                return false;
            }

            /*
             * ---------------------------------------------------------
             * CASE 1
             * Drop sobre un objeto del inventario.
             *
             * Si el objeto es un equipo compatible con el mismo
             * EquipmentType, realizamos un swap.
             * ---------------------------------------------------------
             */
            if (targetSlotIndex.HasValue)
            {
                int targetIndex =
                    targetSlotIndex.Value;

                if (inventory.State.Items.TryGetAt(
                        targetIndex,
                        out ItemInstance targetItem))
                {
                    if (contentResolver.TryResolve(
                            targetItem.BaseDataId,
                            out ItemBaseData targetBaseData) &&
                        targetBaseData != null &&
                        targetBaseData.Category ==
                            ItemCategory.Equipment &&
                        targetBaseData.EquipmentType ==
                            equipmentType)
                    {
                        /*
                         * Retirar el objeto del inventario antes
                         * de equiparlo evita que la misma
                         * ItemInstance quede registrada a la vez
                         * como equipada y dentro del inventario.
                         */
                        if (!inventory.State.Items.TryRemoveAt(
                                targetIndex,
                                out ItemInstance inventoryItem))
                        {
                            return false;
                        }

                        /*
                         * Equipar el objeto del inventario.
                         *
                         * TryEquip() reemplazará automáticamente
                         * al objeto actualmente equipado y devolverá
                         * ese objeto mediante replacedItem.
                         */
                        if (!loadout.TryEquip(
                                inventoryItem,
                                targetBaseData,
                                out ItemInstance loadoutReplacedItem))
                        {
                            /*
                             * Rollback:
                             * devolver el objeto exactamente al
                             * inventario si el equipamiento falla.
                             */
                            inventory.State.Items.TryAdd(
                                inventoryItem);

                            return false;
                        }

                        /*
                         * El objeto anteriormente equipado pasa
                         * ahora al inventario.
                         */
                        if (loadoutReplacedItem == null)
                        {
                            /*
                             * Estado inesperado. Intentamos restaurar
                             * el objeto que acabamos de equipar.
                             */
                            loadout.TryUnequip(
                                equipmentType,
                                out _);

                            inventory.State.Items.TryAdd(
                                inventoryItem);

                            return false;
                        }

                        if (!inventory.State.Items.TryAdd(
                                loadoutReplacedItem))
                        {
                            /*
                             * No debería ocurrir con el inventario
                             * persistente actual porque TryAdd()
                             * no tiene límite de capacidad.
                             *
                             * Aun así, restauramos el estado anterior
                             * si TryAdd() llegara a fallar.
                             */
                            loadout.TryUnequip(
                                equipmentType,
                                out _);

                            inventory.State.Items.TryAdd(
                                inventoryItem);

                            return false;
                        }

                        removedItem =
                            loadoutReplacedItem;

                        replacedItem =
                            inventoryItem;

                        return true;
                    }
                }
            }

            /*
             * ---------------------------------------------------------
             * CASE 2
             * No se soltó sobre un objeto compatible.
             *
             * Simplemente desequipamos el objeto y lo agregamos
             * al final del inventario.
             * ---------------------------------------------------------
             */

            if (!loadout.TryUnequip(
                    equipmentType,
                    out ItemInstance unequippedItem))
            {
                return false;
            }

            if (!inventory.State.Items.TryAdd(
                    unequippedItem))
            {
                /*
                 * Rollback.
                 *
                 * El inventario actual no tiene límite de capacidad,
                 * por lo que normalmente esto nunca debería fallar.
                 */
                if (contentResolver.TryResolve(
                        unequippedItem.BaseDataId,
                        out ItemBaseData baseData) &&
                    baseData != null)
                {
                    loadout.TryEquip(
                        unequippedItem,
                        baseData,
                        out _);
                }

                return false;
            }

            removedItem =
                unequippedItem;

            return true;
        }
    }
}