using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerGroupAssociationUpdatedMessage
    {
        public Identity Identity { get; set; }
        public Group.Shared.Group Group { get; set; }
    }
}
