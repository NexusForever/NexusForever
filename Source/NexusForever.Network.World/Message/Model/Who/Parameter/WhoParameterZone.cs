namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterZone : IWhoParameterData
    {
        public uint WorldZoneId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            WorldZoneId = reader.ReadUInt(15u);
        }
    }
}
