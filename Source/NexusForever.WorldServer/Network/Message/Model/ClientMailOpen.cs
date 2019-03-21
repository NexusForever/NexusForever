using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailOpen, MessageDirection.Client)]
    public class ClientMailOpen : IReadable
    {
       public uint Unknown0 { get; private set; }
       public ulong MailId { get; private set; }

       public void Read(GamePacketReader reader)
       {
           Unknown0  = reader.ReadUInt();
           MailId  = reader.ReadULong();
       }
    }
}
