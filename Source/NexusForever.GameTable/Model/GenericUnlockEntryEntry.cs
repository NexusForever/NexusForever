namespace NexusForever.GameTable.Model
{
    public class GenericUnlockEntryEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string SpriteIcon { get; set; }
        public string SpritePreview { get; set; }
        public uint GenericUnlockTypeEnum { get; set; }
        public uint UnlockObject { get; set; }
    }
}
