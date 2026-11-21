using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ClientAuctionCancel)]
    public class ClientAuctionCancel : IReadable
    {
        public ulong AuctionId { get; private set; }
        public uint Item2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Item2Id = reader.ReadUInt(18u);
            AuctionId = reader.ReadULong();
        }
    }
}
