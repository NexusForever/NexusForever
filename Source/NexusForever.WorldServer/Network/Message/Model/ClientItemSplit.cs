using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemSplit)]
    public class ClientItemSplit : IReadable
    {
        public ulong Guid { get; private set; }
        public ItemLocation Location { get; } = new ItemLocation();
        public uint Count { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid  = reader.ReadULong();
            Location.Read(reader);
            Count = reader.ReadUInt();
        }
    }
}
