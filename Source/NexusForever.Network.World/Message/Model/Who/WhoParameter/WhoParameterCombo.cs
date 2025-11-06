using NexusForever.Game.Static.Entity;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterCombo : IWhoParameterData
    {
        public string SearchString { get; set; }
        public Race RaceId { get; set; }
        public Path  PathId { get; set; }
        public Class ClassId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint WorldZoneId2 { get; set; } // usually a duplicate of WorldZoneId, not sure why this is only 14 bits
 
        public void Read(GamePacketReader reader)
        {
            SearchString = reader.ReadWideString();
            RaceId = reader.ReadEnum<Race>(14u);
            PathId = reader.ReadEnum<Path>(3u);
            ClassId = reader.ReadEnum<Class>(14u);
            WorldZoneId = reader.ReadUInt(15u);
            WorldZoneId2 = reader.ReadUInt(14u);
        }
    }
}
