using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Marketplace;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Marketplace.Filter;

namespace NexusForever.Network.World.Message.Model.Marketplace
{
    [Message(GameMessageOpcode.ClientAuctionsByFilterRequest)]
    public class ClientAuctionsByFilterRequest : IReadable
    {
        // Only one of the four fields below will be filled out depedning on the search type
        // Family, Category, Type, or by item(s)
        public uint Item2FamilyId { get; private set; } 
        public uint Item2CategoryId { get; private set; }
        public uint Item2TypeId { get; private set; }
        public List<uint> Item2Ids { get; private set; } = [];

        public List<IAuctionFilter> Filters { get; private set; } = [];
        public uint ReverseSort { get; private set; } // is a flag array but only lowest bit is used
        public AuctionSort AuctionSort { get; private set; }
        public Property SortPropertyId { get; private set; } // required if AuctionSort is Property
        public uint Page { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Item2FamilyId = reader.ReadUInt(14u);
            Item2CategoryId = reader.ReadUInt(14u);
            Item2TypeId = reader.ReadUInt(14u);

            uint count = reader.ReadUInt();
            for(uint i = 0; i < count; i++)
            {
                Item2Ids.Add(reader.ReadUInt());
            }

            count = reader.ReadUInt();
            for(uint i = 0; i < count; i++)
            {
                AuctionFilter filter = new AuctionFilter();
                filter.Read(reader);
                Filters.Add(filter.Filter);
            }

            ReverseSort = reader.ReadUInt();
            AuctionSort = reader.ReadEnum<AuctionSort>(3u);
            SortPropertyId = reader.ReadEnum<Property>(8u);
            Page = reader.ReadUInt();
        }
    }
}
