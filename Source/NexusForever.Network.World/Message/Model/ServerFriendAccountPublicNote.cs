using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Can also be used to send the player's updated public note
    [Message(GameMessageOpcode.ServerFriendAccountPublicNote)]
    public class ServerFriendAccountPublicNote : IWritable
    {
        public uint AccountId { get; set; }
        public string PublicNote { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.WriteStringWide(PublicNote);
        }
    }
}
