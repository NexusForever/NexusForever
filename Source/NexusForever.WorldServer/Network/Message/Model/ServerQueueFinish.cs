using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerQueueFinish)]
    public class ServerQueueFinish : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
        }
    }
}
