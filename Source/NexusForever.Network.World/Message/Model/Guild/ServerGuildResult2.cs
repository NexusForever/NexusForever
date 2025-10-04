using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildResult2)]
    public class ServerGuildResult2 : ServerGuildResult
    {
        // Client treats opcode the same as ServerGuildResult so just use that message
        // Added here not because it is useful but to document its duplication of function
    }
}
