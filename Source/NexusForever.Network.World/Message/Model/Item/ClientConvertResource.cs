using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientConvertResource)]
    public class ClientConvertResource : IReadable
    {
        public ushort ResourceConversionId { get; private set; }
        public uint SourceQuantity { get; private set; }
        public ulong ItemGuid { get; private set; } // item to convert

        public void Read(GamePacketReader reader)
        {
            ResourceConversionId = reader.ReadUShort(14u);
            SourceQuantity = reader.ReadUInt();
            ItemGuid = reader.ReadULong();
        }
    }
}
