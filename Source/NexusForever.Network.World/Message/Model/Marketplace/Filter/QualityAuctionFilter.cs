using NexusForever.Game.Static.Item;

namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class QualityAuctionFilter : IAuctionFilter
    {
        public Quality Minimum { get; private set; }
        public Quality Maximum { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Minimum = reader.ReadEnum<Quality>(4u);
            Maximum = reader.ReadEnum<Quality>(4u);
        }
    }
}
