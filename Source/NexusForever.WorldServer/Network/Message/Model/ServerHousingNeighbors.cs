using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingNeighbors, MessageDirection.Server)]
    public class ServerHousingNeighbors : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            writer.Write(0);
        }
    }
}
