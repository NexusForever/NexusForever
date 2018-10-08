using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAuthAccepted, MessageDirection.Server)]
    public class ServerAuthAccepted : IWritable
    {
        public uint Unknown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown);
        }
    }
}
