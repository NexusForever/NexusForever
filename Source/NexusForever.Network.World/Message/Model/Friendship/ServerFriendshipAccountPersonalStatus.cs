using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAccountPersonalStatus)]
    public class ServerFriendshipAccountPersonalStatus : IWritable
    {
        public string AccountPublicStatus { get; set; }
        public string AccountNickname { get; set; }
        public AccountPresenceState Presence { get; set; }
        public bool BlockStrangerRequests { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(AccountPublicStatus);
            writer.WriteStringWide(AccountNickname);
            writer.Write(Presence, 3);
            writer.Write(BlockStrangerRequests);
        }
    }
}
