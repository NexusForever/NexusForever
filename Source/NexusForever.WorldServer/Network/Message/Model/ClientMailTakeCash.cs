using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientMailTakeCash)]
    public class ClientMailTakeCash : IReadable
    {
       public ulong MailId { get; private set; }
       public uint UnitId { get; private set; } // Mailbox being interacted with

       public void Read(GamePacketReader reader)
       {
           MailId = reader.ReadULong();
           UnitId = reader.ReadUInt();
       }
    }
}
