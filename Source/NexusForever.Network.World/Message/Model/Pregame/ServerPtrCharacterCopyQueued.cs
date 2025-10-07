using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Can be sent to client while player is in Game or on CharacterSelect screen.
    [Message(GameMessageOpcode.ServerPtrCharacterCopyQueued)]
    public class ServerPtrCharacterCopyQueued : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
