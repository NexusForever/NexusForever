using NexusForever.Network.Message;

namespace NexusForever.Network.World.Model.Loot
{
    [Message(GameMessageOpcode.ServerLootRemove)]
    public class ServerLootRemove : IWritable
    {
        public uint OwnerUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(OwnerUnitId);
        }
    }
}
