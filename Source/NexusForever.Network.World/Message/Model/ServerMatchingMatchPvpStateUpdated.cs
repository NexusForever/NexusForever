using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchPvpStateUpdated)]
    public class ServerMatchingMatchPvpStateUpdated : StateInfo
    {
    }
}
