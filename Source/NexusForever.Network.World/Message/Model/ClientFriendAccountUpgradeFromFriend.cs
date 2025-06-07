using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Takes an existing friend and uses the identity of that character to
    // send an account friend invite.
    [Message(GameMessageOpcode.ClientFriendAccountUpgradeFromFriend)]
    public class ClientFriendAccountUpgradeFromFriend : IReadable
    {
        public TargetPlayerIdentity ExisitingFriend { get; set; } = new();
        public string Note { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ExisitingFriend.Read(reader);
            Note = reader.ReadWideString();
        }
    }
}
