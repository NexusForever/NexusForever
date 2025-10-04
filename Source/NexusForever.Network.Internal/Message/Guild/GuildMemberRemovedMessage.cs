using NexusForever.Game.Static.Guild;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Guild
{
    // TODO: should be updated once GuildServer is implemented
    public class GuildMemberRemovedMessage
    {
        public ulong GuildId { get; set; }
        public GuildType Type { get; set; }
        public Identity Member { get; set; }
    }
}
