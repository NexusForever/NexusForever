using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventPersonalStatsUpdate)]
    public class ServerPublicEventPersonalStatsUpdate : PublicEventStats
    {
        // Not seen in sniffs as there are other options for sending the same data but this is still usable.
    }
}
