using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerLootGrant)]
    public class ServerLootGrant : IWritable
    {
        public uint UnitId { get; set; }
        public uint LooterId { get; set; }
        public LootItem LootItem { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(LooterId);
            LootItem.Write(writer);
        }
    }
}
