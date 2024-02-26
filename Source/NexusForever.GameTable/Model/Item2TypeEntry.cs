using NexusForever.GameTable.Static { get; set; }

namespace NexusForever.GameTable.Model
{
    public class Item2TypeEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint ItemSlotId { get; set; }
        public uint SoundEventIdLoot { get; set; }
        public uint SoundEventIdEquip { get; set; }
        public SecondaryItemFlags Flags { get; set; }
        public float VendorMultiplier { get; set; }
        public float TurninMultiplier { get; set; }
        public uint Item2CategoryId { get; set; }
        public float ReferenceMuzzleX { get; set; }
        public float ReferenceMuzzleY { get; set; }
        public float ReferenceMuzzleZ { get; set; }
    }
}
