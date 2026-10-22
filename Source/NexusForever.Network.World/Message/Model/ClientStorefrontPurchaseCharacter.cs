using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontPurchaseCharacter)]
    public class ClientStorefrontPurchaseCharacter : IReadable
    {
        public uint OfferId { get; set; } // OfferId
        public byte Unknown1 { get; set; }
        public float Unknown2 { get; set; } // the client writes this slot with movss, so it is a float
        public ushort CurrencyId { get; set; } // CurrencyId - 1408 == NCCoin, 768 == Omnibits
        public uint Unknown3 { get; set; }
        public Identity Target { get; set; } = new();
        public uint Unknown6 { get; set; }

        public void Read(GamePacketReader reader)
        {
            OfferId    = reader.ReadUInt();
            Unknown1   = reader.ReadByte(5);
            Unknown2   = reader.ReadSingle();
            CurrencyId = reader.ReadUShort(14);
            Unknown3   = reader.ReadUInt();
            Target.Read(reader);
            Unknown6   = reader.ReadUInt();
        }
    }
}
