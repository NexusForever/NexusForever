using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientLootItem)]
    public class ClientLootItem : IReadable
    {
        public uint OwnerId { get; private set; }
        public uint LootId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            OwnerId = reader.ReadUInt();
            LootId = reader.ReadUInt();
        }
    }
}
