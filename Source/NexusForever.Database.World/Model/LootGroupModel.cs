using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class LootGroupModel
    {
        public ulong Id { get; set; }
        public ulong? ParentId { get; set; }
        public float Probability { get; set; }
        public uint MinDrop { get; set; }
        public uint MaxDrop { get; set; }
        public uint Condition { get; set; }
        public string Comment { get; set; }

        public LootGroupModel Parent { get; set; }
        public ICollection<LootGroupModel> ChildGroup { get; set; } = new HashSet<LootGroupModel>();
        public ICollection<LootItemModel> Item { get; set; } = new HashSet<LootItemModel>();
    }
}
