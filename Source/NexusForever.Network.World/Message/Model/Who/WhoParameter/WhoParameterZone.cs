namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterZone : IWhoParameterData
    {
        public uint WorldZoneId { get; set; }

        public void Read(GamePacketReader reader)
        {
            WorldZoneId = reader.ReadUInt(15u);
        }
    }
}
