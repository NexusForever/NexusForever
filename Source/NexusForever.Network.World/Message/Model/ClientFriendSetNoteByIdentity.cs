using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendSetNoteByIdentity)]
    public class ClientFriendSetNoteByIdentity : IReadable
    {
        public TargetPlayerIdentity PlayerIdentity { get; private set; } = new TargetPlayerIdentity();
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
            Note = reader.ReadWideString();
        }
    }
}
