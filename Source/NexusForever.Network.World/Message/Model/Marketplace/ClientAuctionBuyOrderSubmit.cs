using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ClientAuctionBuyOrderSubmit)]
    public class ClientAuctionBuyOrderSubmit : IReadable
    {
        public uint Item2Id { get; private set; }
        public ulong AuctionId { get; private set; }
        public ulong AmountOffered { get; private set; } // is a buyout if AmountOffered == auction's buyout price

        public void Read(GamePacketReader reader)
        {
            Item2Id = reader.ReadUInt(18u);
            AuctionId = reader.ReadULong();
            AmountOffered = reader.ReadULong();
        }
    }
}
