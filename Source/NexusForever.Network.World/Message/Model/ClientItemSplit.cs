using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemSplit)]
    public class ClientItemSplit : IReadable
    {
        public ulong Guid { get; private set; }
        public ItemLocation Location { get; } = new();
        public uint Count { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid  = reader.ReadULong();
            Location.Read(reader);
            Count = reader.ReadUInt();
        }
    }
}
