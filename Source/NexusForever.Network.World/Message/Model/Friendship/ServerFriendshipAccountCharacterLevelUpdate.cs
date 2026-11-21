using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    [Message(GameMessageOpcode.ServerFriendshipAccountCharacterLevelUpdate)]
    public class ServerFriendshipAccountCharacterLevelUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public Identity Character { get; set; }
        public uint Level { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            Character.Write(writer);
            writer.Write(Level, 8u);
        }
    }
}
