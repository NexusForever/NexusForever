using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmBroadcast, MessageDirection.Server)]
    public class ServerRealmBroadcast : IWritable
    {
        public byte Tier { get; set; } // 0 = Notification Windows & Chat Message; 1 = Notification Message centre screen & Chat Message; 2 = Chat Message
        public string Message { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tier, 2);
            writer.WriteStringWide(Message);
        }
    }
}
