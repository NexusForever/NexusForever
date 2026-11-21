using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAccountLastOnlineUpdate)]
    public class ServerFriendshipAccountLastOnlineUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public float DaysSinceLastOnline { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(DaysSinceLastOnline);
        }
    }
}
