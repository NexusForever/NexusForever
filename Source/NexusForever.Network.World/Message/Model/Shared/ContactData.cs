using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ContactData : IWritable
    {
        public ulong ContactId { get; set; }
        public TargetPlayerIdentity PlayerIdentity { get; set; } = new TargetPlayerIdentity();
        public string Note { get; set; } = "";
        public ContactType Type { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ContactId);
            PlayerIdentity.Write(writer);
            writer.WriteStringWide(Note);
            writer.Write(Type, 4u);
        }
    }
}
