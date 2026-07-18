using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class EntityLootModel
    {
        public uint Id { get; set; }
        public ulong? LootGroupId { get; set; }
        public string Comment { get; set; }

        public LootGroupModel LootGroup { get; set; }
    }
}
