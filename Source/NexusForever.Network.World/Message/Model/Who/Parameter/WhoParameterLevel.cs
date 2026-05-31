namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterLevel : IWhoParameterData
    {
        public uint BottomLevel { get; private set; }
        public uint TopLevel { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BottomLevel = reader.ReadUInt();
            TopLevel = reader.ReadUInt();
        }
    }
}
