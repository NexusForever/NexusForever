
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingHarvestItemsSentToOwner)]
    public class ServerHousingHarvestItemsSentToOwner : IWritable
    {
        public class HarvestItem
        {
            public uint Item2Id { get; set; }
            public uint Count { get; set; }
        }

        public List<HarvestItem> HarvestedItems { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(HarvestedItems.Count);
            HarvestedItems.ForEach(h => writer.Write(h.Item2Id));
            HarvestedItems.ForEach(h => writer.Write(h.Count));
        }
    }
}
