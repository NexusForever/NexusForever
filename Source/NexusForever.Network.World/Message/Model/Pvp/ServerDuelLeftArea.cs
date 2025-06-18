using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    [Message(GameMessageOpcode.ServerDuelLeftArea)]
    public class ServerDuelLeftArea : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
