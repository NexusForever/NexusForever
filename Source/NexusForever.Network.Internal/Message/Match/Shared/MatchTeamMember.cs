using NexusForever.Game.Static.Matching;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Match.Shared
{
    public class MatchTeamMember
    {
        public Identity Identity { get; set; }
        public Role Roles { get; set; }
    }
}
