using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityStatUpdateInt)]
    public class ServerEntityStatUpdateInt : ServerEntityStatUpdateFloat
    {
    }
}
