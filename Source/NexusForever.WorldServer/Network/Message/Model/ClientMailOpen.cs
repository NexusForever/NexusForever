using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailOpen)]
    public class ClientMailOpen : IReadable
    {
       public uint Count { get; private set; }
        public List<ulong> MailList { get; private set; } = new List<ulong>();

       public void Read(GamePacketReader reader)
       {
            Count  = reader.ReadUInt();
            for(int i = 0; i < Count; i++)
                MailList.Add(reader.ReadULong());
       }
    }
}
