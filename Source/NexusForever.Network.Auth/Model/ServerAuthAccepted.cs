using NexusForever.Network.Message;

namespace NexusForever.Network.Auth.Model
{
    [Message(GameMessageOpcode.ServerAuthAccepted)]
    public class ServerAuthAccepted : IWritable
    {
        public uint Unknown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown);
        }
    }
}
