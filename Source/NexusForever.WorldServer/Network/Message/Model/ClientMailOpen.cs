using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailOpen)]
    public class ClientMailOpen : IReadable
    {
        public List<ulong> MailList { get; } = new List<ulong>();

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (int i = 0; i < count; i++)
                MailList.Add(reader.ReadULong());
        }
    }
}
