using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PinataLootEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public uint ItemId { get; set; }
        public uint ItemCount { get; set; }
        public byte LootType { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(ItemId);
            writer.Write(ItemCount);
            writer.Write(LootType, 4);
        }
    }
}
