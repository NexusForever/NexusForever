using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendLevelUpdate)]
    public class ServerFriendLevelUpdate : IWritable
    {
        public Identity Character { get; set; }
        public uint Level { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Character.Write(writer);
            writer.Write(Level);
        }
    }
}
