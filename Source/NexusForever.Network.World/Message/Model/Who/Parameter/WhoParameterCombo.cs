using NexusForever.Game.Static.Entity;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterCombo : IWhoParameterData
    {
        public string SearchString { get; private set; }
        public Race RaceId { get; private set; }
        public Path PathId { get; private set; }
        public Class ClassId { get; private set; }
        public ushort WorldZoneId { get; private set; }
        public ushort WorldZoneId2 { get; private set; } // usually a duplicate of WorldZoneId, not sure why this is only 14 bits
 
        public void Read(GamePacketReader reader)
        {
            SearchString = reader.ReadWideString();
            RaceId = reader.ReadEnum<Race>(14u);
            PathId = reader.ReadEnum<Path>(3u);
            ClassId = reader.ReadEnum<Class>(14u);
            WorldZoneId = reader.ReadUShort(15u);
            WorldZoneId2 = reader.ReadUShort(14u);
        }
    }
}
