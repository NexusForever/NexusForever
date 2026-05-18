using System.Numerics;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerPositionUpdatedMessage
    {
        public Identity Identity { get; set; }
        public Position Position { get; set; }
        //public Position Rotation { get; set; }
    }
}
