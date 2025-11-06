namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterPath : IWhoParameterData
    {
        public uint PathId { get; set; }

        public void Read(GamePacketReader reader)
        {
            PathId = reader.ReadUInt(3u);
        }
    }
}
