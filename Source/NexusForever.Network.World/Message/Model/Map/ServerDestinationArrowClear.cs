using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Map
{
    // Clears the player's destination arrow regardless of how it was set
    [Message(GameMessageOpcode.ServerDestinationArrowClear)]
    public class ServerDestinationArrowClear : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // zero byte message
        }
    }
}
