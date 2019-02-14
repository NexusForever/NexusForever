using NexusForever.Game.Static.Contact;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerInfoRequest)]
    public class ClientPlayerInfoRequest : IReadable
    {
        public ContactType Type { get; private set; }
        public TargetPlayerIdentity Identity { get; } = new();

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<ContactType>(4u);
            Identity.Read(reader);
        }
    }
}
