namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterGuild : IWhoParameterData
    {
        public string GuildName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GuildName = reader.ReadWideString();
        }
    }
}
