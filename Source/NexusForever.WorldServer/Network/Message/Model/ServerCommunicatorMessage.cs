using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCommunicatorMessage)]
    public class ServerCommunicatorMessage : IWritable
    {
        public ushort CommunicatorId { get; set; }
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CommunicatorId, 15u);
            writer.Write(Unknown0);
        }
    }
}
