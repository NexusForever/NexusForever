using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMailUnavailable, MessageDirection.Server)]
    public class ServerMailUnavailable : IWritable
    {
        public ulong MailId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MailId);
        }
    }
}
