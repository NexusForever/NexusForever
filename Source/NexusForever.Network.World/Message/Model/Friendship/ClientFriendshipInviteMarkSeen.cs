using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipInviteMarkSeen)]
    public class ClientFriendshipInviteMarkSeen : IReadable
    {
        public List<ulong> FriendInviteIds { get; private set; } = [];// Array of InviteIds

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt(16);
            for (int i = 0; i < count; i++)
                FriendInviteIds.Add(reader.ReadULong());
        }
    }
}
