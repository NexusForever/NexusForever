using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontPurchaseCharacter)]
    public class ClientStorefrontPurchaseCharacter : IReadable
    {
        public uint OfferId { get; private set; } // OfferId
        public byte Unknown1 { get; private set; }
        public ushort CurrencyId { get; private set; } // CurrencyId - 1408 == NCCoin, 768 == Omnibits
        public uint Unknown3 { get; private set; }
        public Identity Target { get; set; } = new();
        public uint Unknown6 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            OfferId    = reader.ReadUInt(20);
            Unknown1   = reader.ReadByte(5);
            CurrencyId = reader.ReadUShort(14);
            Unknown3   = reader.ReadUInt(20);
            Target.Read(reader);
            Unknown6   = reader.ReadUInt(20);
        }
    }
}
