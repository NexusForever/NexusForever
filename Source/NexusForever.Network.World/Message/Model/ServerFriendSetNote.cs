using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendSetNote)]
    public class ServerFriendSetNote : IWritable
    {
        public ulong FriendshipId { get; set; }
        public string Note { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FriendshipId);
            writer.WriteStringWide(Note);
        }
    }
}
