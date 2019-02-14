using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsSetNote)]
    public class ServerContactsSetNote : IWritable
    {
        public ulong ContactId { get; set; }
        public string Note { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ContactId);
            writer.WriteStringWide(Note);
        }
    }
}
