using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupJoin)]
    public class ServerGroupJoin : IWritable
    {
        public Identity TargetPlayer { get; set; } = new();
        public GroupInfo GroupInfo { get; set; } = new GroupInfo();

        public void Write(GamePacketWriter writer)
        {
            TargetPlayer.Write(writer);
            GroupInfo.Write(writer);
        }
    }
}
