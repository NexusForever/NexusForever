using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Network.World.Message.Model.Marketplace.Filter
{
    public class EquippableByAuctionFilter : IAuctionFilter
    {
        public Race RaceId { get; private set; }
        public Class ClassId { get; private set; }
        public Faction FactionId { get; private set; }
        public uint Level { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RaceId = reader.ReadEnum<Race>(14u);
            ClassId = reader.ReadEnum<Class>(14u);
            FactionId = reader.ReadEnum<Faction>(14u);
            Level = reader.ReadUInt();
        }
    }
}
