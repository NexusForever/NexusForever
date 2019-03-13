using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
//    [Message(GameMessageOpcode.ServerChangeInnate, MessageDirection.Server)]
    public class ServerChangeInnate : IWritable
    {
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
        }
    }
}
