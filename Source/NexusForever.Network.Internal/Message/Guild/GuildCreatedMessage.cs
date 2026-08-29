using NexusForever.Game.Static.Guild;

namespace NexusForever.Network.Internal.Message.Guild
{
    // TODO: should be updated once GuildServer is implemented
    public class GuildCreatedMessage
    {
        public ulong GuildId { get; set; }
        public GuildType Type { get; set; }
    }
}
