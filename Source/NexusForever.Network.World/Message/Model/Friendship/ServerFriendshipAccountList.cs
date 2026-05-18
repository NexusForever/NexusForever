using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAccountList)]
    public class ServerFriendshipAccountList : IWritable
    {
        public class AccountFriend : IWritable
        {
            public uint AccountId { get; set; }
            public ulong AccountFriendId { get; set; }
            public string PublicNote { get; set; } = "";
            public float DaysSinceLastLogin { get; set; }
            public string PrivateNote { get; set; } = "";
            public string DisplayName { get; set; } = "";
            public AccountPresenceState Presence { get; set; }
            public List<CharacterData> CharacterList { get; set; } = [];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(AccountId);
                writer.Write(AccountFriendId);
                writer.WriteStringWide(PublicNote);
                writer.Write(DaysSinceLastLogin);
                writer.WriteStringWide(PrivateNote);
                writer.WriteStringWide(DisplayName);
                writer.Write(Presence, 3u);

                writer.Write(CharacterList.Count, 32u);
                CharacterList.ForEach(c => c.Write(writer));
            }
        }

        public List<AccountFriend> FriendListData { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendListData.Count, 32u);
            FriendListData.ForEach(f => f.Write(writer));
        }
    }
}
