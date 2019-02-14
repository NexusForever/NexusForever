using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsRequestRemove)]
    public class ServerContactsRequestRemove : IWritable
    {
        public ulong ContactId { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ContactId);
        }
    }
}
