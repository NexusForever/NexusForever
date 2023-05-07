using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsAccountList)]
    public class ServerContactsAccountList : IWritable
    {
        public class AccountFriend : IWritable
        {
            public uint AccountIdFriend { get; set; }
            public ulong FriendRecordId { get; set; }
            public string PublicNote { get; set; } = "";
            public int DaysSinceLastLogin { get; set; }
            public string PrivateNote { get; set; } = "";
            public string DisplayName { get; set; } = "";
            public ChatPresenceState Presence { get; set; }

            public List<CharacterData> CharacterList = new List<CharacterData>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(AccountIdFriend);
                writer.Write(FriendRecordId);
                writer.WriteStringWide(PublicNote);
                writer.Write(DaysSinceLastLogin);
                writer.WriteStringWide(PrivateNote);
                writer.WriteStringWide(DisplayName);
                writer.Write(Presence, 3u);

                writer.Write(CharacterList.Count, 32u);
                CharacterList.ForEach(c => c.Write(writer));
            }
        }

        public List<AccountFriend> FriendListData { get; set; } = new List<AccountFriend>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendListData.Count, 32u);
            FriendListData.ForEach(f => f.Write(writer));
        }
    }
}
