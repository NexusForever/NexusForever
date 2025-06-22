using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Can also be used to send the player their Presence state
    [Message(GameMessageOpcode.ServerFriendAccountPresenceUpdate)]
    public class ServerFriendAccountPresenceUpdate : IWritable
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
