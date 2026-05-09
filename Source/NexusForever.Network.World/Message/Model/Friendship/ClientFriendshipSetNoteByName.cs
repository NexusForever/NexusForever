using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipSetNoteByName)]
    public class ClientFriendshipSetNoteByName : IReadable
    {
        public string Name { get; private set; }
        public string RealmName { get; private set; }
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Name      = reader.ReadWideString();
            RealmName = reader.ReadWideString();
            Note      = reader.ReadWideString();
        }
    }
}
