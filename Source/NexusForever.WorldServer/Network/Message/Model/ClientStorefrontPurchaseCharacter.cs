using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontPurchaseCharacter)]
    public class ClientStorefrontPurchaseCharacter : IReadable
    {
        public uint OfferId { get; set; } // OfferId
        public byte Unknown1 { get; set; }
        public ushort CurrencyId { get; set; } // CurrencyId - 1408 == NCCoin, 768 == Omnibits
        public uint Unknown3 { get; set; }
        public TargetPlayerIdentity Target { get; set; } = new TargetPlayerIdentity();
        public uint Unknown6 { get; set; }

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
