using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientContactsRequestResponse)]
    public class ClientContactsRequestResponse : IReadable
    {
        public ulong ContactId { get; private set; }
        public ContactResponse Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ContactId  = reader.ReadULong(64u);
            Response  = (ContactResponse)reader.ReadByte(3u);
        }
    }
}
