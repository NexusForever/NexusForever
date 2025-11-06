namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterGuild : IWhoParameterData
    {
        public string GuildName { get; set; }

        public void Read(GamePacketReader reader)
        {
            GuildName = reader.ReadWideString();
        }
    }
}
