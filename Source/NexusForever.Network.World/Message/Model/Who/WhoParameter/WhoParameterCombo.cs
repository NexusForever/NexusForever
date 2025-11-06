namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterCombo : IWhoParameterData
    {
        public string SearchString { get; set; }
        public uint RaceId { get; set; }
        public uint PathId { get; set; }
        public uint ClassId { get; set; }
        public uint WorldZoneId { get; set; }
        public uint WorldZoneId2 { get; set; } // usually a duplicate of WorldZoneId, not sure why this is only 14 bits
 
        public void Read(GamePacketReader reader)
        {
            SearchString = reader.ReadWideString();
            RaceId = reader.ReadUInt(14u);
            PathId = reader.ReadUInt(3u);
            ClassId = reader.ReadUInt(14u);
            WorldZoneId = reader.ReadUInt(15u);
            WorldZoneId2 = reader.ReadUInt(14u);
        }
    }
}
