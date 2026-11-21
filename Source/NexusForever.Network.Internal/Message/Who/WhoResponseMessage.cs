using NexusForever.Game.Static.Who;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Who
{
    public class WhoResponseMessage
    {
        public Identity Identity { get; set; }
        public WhoResult Result { get; set; }
        public List<WhoCharacter> Characters { get; set; } = [];
    }
}
