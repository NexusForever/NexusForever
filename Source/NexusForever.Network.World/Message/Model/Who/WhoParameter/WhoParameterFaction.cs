namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterFaction : IWhoParameterData
    {
        public uint Faction2Id { get; set; }

        public void Read(GamePacketReader reader)
        {
            Faction2Id = reader.ReadUInt(14u);
        }
    }
}
