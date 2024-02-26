namespace NexusForever.GameTable.Model
{
    public class GenericMapNodeEntry
    {
        public uint Id { get; set; }
        public uint GenericMapId { get; set; }
        public uint WorldLocation2Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string SpritePath { get; set; }
        public uint GenericMapNodeTypeEnum { get; set; }
        public uint Flags { get; set; }
    }
}
