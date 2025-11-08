using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityStatUpdateInteger)]
    public class ServerEntityStatUpdateInteger : ServerEntityStatUpdateFloat
    {
    }
}
