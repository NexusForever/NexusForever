using NexusForever.Game.Abstract.Guild;

namespace NexusForever.Game.Guild
{
    public class GuildInvite : IGuildInvite
    {
        public ulong GuildId { get; set; }
        public ulong InviteeId { get; set; }
    }
}
