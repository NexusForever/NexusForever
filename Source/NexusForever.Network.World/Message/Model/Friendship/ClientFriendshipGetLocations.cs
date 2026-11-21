using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ClientFriendshipGetLocations)]
    public class ClientFriendshipGetLocations : IReadable
    {
        public List<Identity> FriendIdentities { get; private set; } = []; // Identities of friends that are not account friends
        public List<ulong> AccountFriendIds { get; private set; } = []; // For friends that are account friends

        public void Read(GamePacketReader reader)
        {
            uint friendCount = reader.ReadUInt(8);
            for (int i = 0; i < friendCount; i++)
            {
                var identity = new Identity();
                identity.Read(reader);
                FriendIdentities.Add(identity);
            }

            uint accountFriendsCount = reader.ReadUInt(8);
            for (int i = 0; i < accountFriendsCount; i++)
                AccountFriendIds.Add(reader.ReadULong());
        }
    }
}
