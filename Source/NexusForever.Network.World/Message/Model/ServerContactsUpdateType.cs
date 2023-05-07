using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsUpdateType)]
    public class ServerContactsUpdateType : IWritable
    {
        public ulong ContactId { get; set; }
        public ContactType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ContactId);
            writer.Write(Type, 4u);
        }
    }
}
