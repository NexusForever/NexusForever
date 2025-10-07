using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class PropertyMinAuctionFilter : IAuctionFilter
    {
        public Property UnitProperty2Id { get; private set; }
        public float Minimum { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnitProperty2Id = reader.ReadEnum<Property>(8u);
            Minimum = reader.ReadSingle();
        }
    }
}
