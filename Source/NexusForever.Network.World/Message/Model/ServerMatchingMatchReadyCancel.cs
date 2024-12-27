using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchReadyCancel)]
    public class ServerMatchingMatchReadyCancel : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // deliberately empty
        }
    }
}
