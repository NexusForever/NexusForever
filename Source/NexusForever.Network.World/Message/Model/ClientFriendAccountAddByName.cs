using NexusForever.Game.Static.Friend;
using NexusForever.Network;
using NexusForever.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientFriendAccountAddByName)]
    public class ClientFriendAccountAddByName : IReadable
    {
        public string PlayerName { get; private set; }
        public string RealmName { get; private set; }
        public FriendshipType Type { get; private set; }
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerName  = reader.ReadWideString();
            RealmName  = reader.ReadWideString();
            Type  = reader.ReadEnum<FriendshipType>(4);
            Note  = reader.ReadWideString();
        }
    }
}
