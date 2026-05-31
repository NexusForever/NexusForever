using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    // Takes an existing friend and uses the identity of that character to
    // send an account friend invite.
    [Message(GameMessageOpcode.ClientFriendshipAccountUpgradeFromFriend)]
    public class ClientFriendshipAccountUpgradeFromFriend : IReadable
    {
        public Identity ExisitingFriend { get; private set; } = new();
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ExisitingFriend.Read(reader);
            Note = reader.ReadWideString();
        }
    }
}
