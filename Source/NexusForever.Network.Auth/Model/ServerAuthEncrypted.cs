using NexusForever.Network.Message;

namespace NexusForever.Network.Auth.Model
{
    [Message(GameMessageOpcode.ServerAuthEncrypted)]
    public class ServerAuthEncrypted : IWritable
    {
        public byte[] Data { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Data.Length + 4);
            writer.WriteBytes(Data);
        }
    }
}
