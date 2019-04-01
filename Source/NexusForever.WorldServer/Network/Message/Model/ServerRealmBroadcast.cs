using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmBroadcast, MessageDirection.Server)]
    public class ServerRealmBroadcast : IWritable
    {
        public byte Unknown0 { get; set; } // 2
        public string Message { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 2);
            writer.WriteStringWide(Message);
        }
    }
}
