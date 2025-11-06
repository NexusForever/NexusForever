namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterLevel : IWhoParameterData
    {
        public uint BottomLevel { get; set; }
        public uint TopLevel { get; set; }

        public void Read(GamePacketReader reader)
        {
            BottomLevel = reader.ReadUInt();
            TopLevel = reader.ReadUInt();
        }
    }
}
