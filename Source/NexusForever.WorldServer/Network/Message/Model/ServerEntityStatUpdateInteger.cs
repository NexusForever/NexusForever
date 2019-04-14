using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityStatUpdateInteger)]
    public class ServerEntityStatUpdateInteger : ServerEntityStatUpdateFloat
    {
    }
}
