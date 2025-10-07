namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class LevelAuctionFilter : IAuctionFilter
    {
        public uint Minimum { get; private set; }
        public uint Maximum { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Minimum = reader.ReadUInt();
            Maximum = reader.ReadUInt();
        }
    }
}
