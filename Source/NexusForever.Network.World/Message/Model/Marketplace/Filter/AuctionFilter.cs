using NexusForever.Game.Static.Marketplace;

namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class AuctionFilter : IAuctionFilter
    {
        public AuctionFilterType Type { get; private set; }
        public IAuctionFilter Filter { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<AuctionFilterType>(3u);

            Filter = null;

            switch (Type)
            {
                case AuctionFilterType.PropertyMin:
                    Filter = new PropertyMinAuctionFilter();
                    break;
                case AuctionFilterType.PropertyMax:
                    Filter = new PropertyMaxAuctionFilter();
                    break;
                case AuctionFilterType.Level:
                    Filter = new LevelAuctionFilter();
                    break;
                case AuctionFilterType.Quality:
                    Filter = new QualityAuctionFilter();
                    break;
                case AuctionFilterType.RuneSlot:
                    Filter = new RuneSlotAuctionFilter();
                    break;
                case AuctionFilterType.BuyoutMax:
                    Filter = new BuyoutMaxAuctionFilter();
                    break;
                case AuctionFilterType.EquippableBy:
                    Filter = new EquippableByAuctionFilter();
                    break;
            }

            if (Filter == null)
                throw new NotImplementedException();

            Filter.Read(reader);
        }
    }
}
