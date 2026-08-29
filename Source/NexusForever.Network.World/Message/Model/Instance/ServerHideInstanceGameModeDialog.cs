using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    // Fired when the server wants to close the instance window after a ShowInstanceGameModeDialog event.
    [Message(GameMessageOpcode.ServerHideInstanceGameModeDialog)]
    public class ServerHideInstanceGameModeDialog : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
