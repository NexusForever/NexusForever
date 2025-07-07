using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client only processes message when on CharacterSelect screen.
    [Message(GameMessageOpcode.ServerQueueFinish)]
    public class ServerQueueFinish : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
