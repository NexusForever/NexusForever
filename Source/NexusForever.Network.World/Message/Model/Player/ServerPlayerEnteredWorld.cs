using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPlayerEnteredWorld)]
    public class ServerPlayerEnteredWorld : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
