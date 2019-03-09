using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmMessages, MessageDirection.Server)]
    public class ServerRealmMessages : IWritable
    {
        public class Message : IWritable
        {
            public uint Index { get; set; }
            public List<string> Messages { get; set; } = new List<string>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Index);
                writer.Write(Messages.Count, 8);
                Messages.ForEach(writer.WriteStringWide);
            }
        }

        public List<Message> Messages { get; set; } = new List<Message>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Messages.Count);
            Messages.ForEach(m => m.Write(writer));
        }
    }
}
