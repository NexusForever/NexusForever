namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterClass : IWhoParameterData
    {
        public uint ClassId { get; set; }

        public void Read(GamePacketReader reader)
        {
            ClassId = reader.ReadUInt(14u);
        }
    }
}
