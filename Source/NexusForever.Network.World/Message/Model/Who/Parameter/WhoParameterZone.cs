namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterZone : IWhoParameterData
    {
        public ushort WorldZoneId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            WorldZoneId = reader.ReadUShort(15u);
        }
    }
}
