using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class StorefrontPurchase : IReadable
    {
        public uint OfferId { get; set; } // OfferId
        public AccountCurrencyType CurrencyType { get; set; }
        public float Cost { get; set; }
        public ushort Unknown0 { get; set; } // CurrencyId - 1408 == NCCoin, 768 == Omnibits
        public uint Unknown1 { get; set; }
        public TargetPlayerIdentity Target { get; set; } = new TargetPlayerIdentity();
        public uint Unknown2 { get; set; }

        public void Read(GamePacketReader reader)
        {
            OfferId      = reader.ReadUInt();
            CurrencyType = reader.ReadEnum<AccountCurrencyType>(5);
            Cost         = reader.ReadSingle();
            Unknown0     = reader.ReadUShort(14);
            Unknown1     = reader.ReadUInt();
            Target.Read(reader);
            Unknown2     = reader.ReadUInt();
        }
    }
}
