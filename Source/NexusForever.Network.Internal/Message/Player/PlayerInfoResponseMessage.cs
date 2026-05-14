using NexusForever.Game.Static.Info;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerInfoResponseMessage
    {
        public Identity Source { get; set; }
        public Identity Target { get; set; }
        public PlayerInfoRequestType Type { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
    }
}
