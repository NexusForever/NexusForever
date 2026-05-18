using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupJoin)]
    public class ServerGroupJoin : IWritable
    {
        public Identity TargetPlayer { get; set; } = new();
        public Group Group { get; set; } = new Group();

        public void Write(GamePacketWriter writer)
        {
            TargetPlayer.Write(writer);
            Group.Write(writer);
        }
    }
}
