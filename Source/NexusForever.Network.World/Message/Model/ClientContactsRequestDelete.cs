using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientContactsRequestDelete)]
    public class ClientContactsRequestDelete : IReadable
    {
        public TargetPlayerIdentity PlayerIdentity { get; private set; } = new TargetPlayerIdentity();
        public ContactType Type { get; private set; }
        public byte Unknown1 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
            Type = (ContactType)reader.ReadByte(4u);
            Unknown1 = reader.ReadByte(4u);
        }
    }
}
