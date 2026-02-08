using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Chat.Shared
{
    public class ChatCharacter
    {
        public Identity Identity { get; set; }
        public IdentityName IdentityName { get; set; }
        public Faction Faction { get; set; }
        public bool IsOnline { get; set; }
    }
}
