using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemSplit)]
    public class ClientItemSplit : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public ItemLocation Location { get; } = new();
        public uint Count { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            Location.Read(reader);
            Count = reader.ReadUInt();
        }
    }
}
