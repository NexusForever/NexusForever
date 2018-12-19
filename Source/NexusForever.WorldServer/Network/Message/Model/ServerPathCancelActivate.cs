using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathCancelActivate, MessageDirection.Server)]
    public class ServerPathCancelActivate : IWritable
    {
        public byte Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Reason);
        }
    }
}
