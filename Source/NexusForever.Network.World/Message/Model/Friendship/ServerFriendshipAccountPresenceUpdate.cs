using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    // Can also be used to send the player their Presence state
    [Message(GameMessageOpcode.ServerFriendshipAccountPresenceUpdate)]
    public class ServerFriendshipAccountPresenceUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public AccountPresenceState Presence { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(Presence, 3);
        }
    }
}
