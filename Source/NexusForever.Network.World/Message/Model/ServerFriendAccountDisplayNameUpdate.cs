using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Can be used for AccountFriends or the player when they update their account display name
    [Message(GameMessageOpcode.ServerFriendAccountDisplayNameUpdate)]
    public class ServerFriendAccountDisplayNameUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public string DisplayName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.WriteStringWide(DisplayName);
        }
    }
}
