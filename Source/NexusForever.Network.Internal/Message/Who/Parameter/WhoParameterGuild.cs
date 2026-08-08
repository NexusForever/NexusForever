using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterGuild : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Guild;

        public string GuildName { get; set; }
    }
}
