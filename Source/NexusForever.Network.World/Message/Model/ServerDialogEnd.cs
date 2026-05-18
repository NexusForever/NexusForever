using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Not captured in any sniffs so maybe was not needed
    [Message(GameMessageOpcode.ServerDialogEnd)]
    public class ServerDialogEnd : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}
