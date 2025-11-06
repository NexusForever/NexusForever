namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterRace : IWhoParameterData
    {
        public uint RaceId { get; set; }

        public void Read(GamePacketReader reader)
        {
            RaceId = reader.ReadUInt(14u);
        }
    }
}
