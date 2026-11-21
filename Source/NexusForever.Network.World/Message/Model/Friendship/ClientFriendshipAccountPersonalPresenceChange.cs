using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipAccountPersonalPresenceChange)]
    public class ClientFriendshipAccountPersonalPresenceChange : IReadable
    {
        public AccountPresenceState Presence { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Presence = reader.ReadEnum<AccountPresenceState>(3u);
        }
    }
}
