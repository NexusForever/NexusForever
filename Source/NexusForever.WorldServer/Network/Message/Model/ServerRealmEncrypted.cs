using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmEncrypted)]
    public class ServerRealmEncrypted : IWritable
    {
        public byte[] Data { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Data.Length + 4);
            writer.WriteBytes(Data);
        }
    }
}
