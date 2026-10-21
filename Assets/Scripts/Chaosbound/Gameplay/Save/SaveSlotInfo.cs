namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveSlotInfo
    {
        public int SlotIndex { get; }
        public int SlotNumber => SlotIndex + 1;
        public SaveSlotStatus Status { get; }

        public bool HasSave =>
            Status == SaveSlotStatus.Available;

        public bool IsEmpty =>
            Status == SaveSlotStatus.Empty;

        public bool IsInvalid =>
            Status == SaveSlotStatus.Invalid;

        public SaveSlotInfo(
            int slotIndex,
            SaveSlotStatus status)
        {
            SlotIndex = slotIndex;
            Status = status;
        }
    }
}