using NexusForever.Game.Static.Friend;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountInviteList)]
    public class ServerFriendAccountInviteList : IWritable
    {
        public class FriendAccountInviteInfo : IWritable
        {
            public ulong AccountFriendInviteId { get; set; }
            public FriendshipType Type { get; set; }
            public string DisplayName { get; set; }
            public string Note { get; set; }
            public float DaysUntilExpired { get; set; }
            public uint Seen { get; set; } // Could be a whole range of flags, but it seems to only be used for flagging seen invites
            
            public void Write(GamePacketWriter writer)
            {
                writer.Write(AccountFriendInviteId);
                writer.Write(Type, 2u);
                writer.WriteStringWide(DisplayName);
                writer.WriteStringWide(Note);
                writer.Write(DaysUntilExpired);
                writer.Write(Seen);
            }
        }

        public uint AccountId { get; set; }
        public List<FriendAccountInviteInfo> FriendAccountInvites { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(FriendAccountInvites.Count);
            foreach (var invite in FriendAccountInvites)
            {
                invite.Write(writer);
            }
        }
    }
}
