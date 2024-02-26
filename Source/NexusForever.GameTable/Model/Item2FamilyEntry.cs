using NexusForever.GameTable.Static { get; set; }

namespace NexusForever.GameTable.Model
{
    public class Item2FamilyEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextId { get; set; }
        public SecondaryItemFlags Flags { get; set; }
        public uint SoundEventIdEquip { get; set; }
        public float VendorMultiplier { get; set; }
        public float TurninMultiplier { get; set; }
    }
}
