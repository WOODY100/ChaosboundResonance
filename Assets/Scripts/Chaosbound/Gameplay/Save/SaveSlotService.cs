using System;
using Chaosbound.Core.Composition;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveSlotService
    {
        public const int SlotCount = 3;

        private readonly SaveGameService[] saveServices;

        public SaveSlotService()
        {
            saveServices = new SaveGameService[SlotCount];

            for (int i = 0; i < SlotCount; i++)
            {
                string fileName =
                    $"chaosbound_save_{i + 1}.json";

                ISaveStorage storage =
                    new FileSaveStorage(fileName);

                saveServices[i] =
                    new SaveGameService(storage, 1);
            }
        }

        public bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 &&
                   slotIndex < SlotCount;
        }

        public SaveGameService GetService(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    "Save slot index must be between 0 and 2.");
            }

            return saveServices[slotIndex];
        }

        public bool HasSaveFile(int slotIndex)
        {
            return GetService(slotIndex).HasSaveFile();
        }

        public void DeleteSave(int slotIndex)
        {
            GetService(slotIndex).DeleteSave();
        }

        public bool HasValidSave(int slotIndex)
        {
            return GetService(slotIndex).HasValidSave();
        }

        public SaveSlotStatus GetSlotStatus(int slotIndex)
        {
            if (!HasSaveFile(slotIndex))
            {
                return SaveSlotStatus.Empty;
            }

            if (HasValidSave(slotIndex))
            {
                return SaveSlotStatus.Available;
            }

            return SaveSlotStatus.Invalid;
        }

        public SaveSlotInfo GetSlotInfo(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    "Save slot index must be between 0 and 2.");
            }

            SaveSlotStatus status = GetSlotStatus(slotIndex);

            return new SaveSlotInfo(
                slotIndex,
                status);
        }

        public SaveSlotInfo[] GetAllSlotInfo()
        {
            SaveSlotInfo[] slots =
                new SaveSlotInfo[SlotCount];

            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = GetSlotInfo(i);
            }

            return slots;
        }
    }
}