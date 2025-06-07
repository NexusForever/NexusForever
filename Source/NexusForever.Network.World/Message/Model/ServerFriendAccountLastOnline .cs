using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountLastOnline)]
    public class ServerFriendAccountLastOnline : IWritable
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
