namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class RuneSlotAuctionFilter : IAuctionFilter
    {
        public byte MichrochipType { get; private set; }
        public byte Minimum { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MichrochipType = reader.ReadByte(5u);
            Minimum = reader.ReadByte();
        }
    }
}
