using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    // TODO: update this once Guild server is implemented
    public class PlayerGuildAssociationUpdatedMessage
    {
        public Identity Identity { get; set; }
        public string GuildName { get; set; }
    }
}
