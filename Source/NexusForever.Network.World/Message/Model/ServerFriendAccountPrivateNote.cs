using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Can also be used to send the player's updated private note
    [Message(GameMessageOpcode.ServerFriendAccountPrivateNote)]
    public class ServerFriendAccountPrivateNote : IWritable
    {
        public uint AccountId { get; set; }
        public string PrivateNote { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.WriteStringWide(PrivateNote);
        }
    }
}
