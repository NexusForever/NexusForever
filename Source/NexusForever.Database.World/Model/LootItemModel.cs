namespace NexusForever.Database.World.Model
{
    public class LootItemModel
    {
        public ulong Id { get; set; }
        public uint Type { get; set; }
        public uint StaticId { get; set; }
        public float Probability { get; set; }
        public uint MinCount { get; set; }
        public uint MaxCount { get; set; }
        public string Comment { get; set; }

        public LootGroupModel LootGroup { get; set; }
    }
}
