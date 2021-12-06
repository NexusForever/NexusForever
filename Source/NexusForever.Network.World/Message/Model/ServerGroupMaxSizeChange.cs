using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMaxSizeChange)]
    public class ServerGroupMaxSizeChange : IWritable
    {
        public ulong GroupId { get; set; }

        public GroupFlags NewFlags { get; set; }

        public uint NewMaxSize { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(NewFlags, 32u);
            writer.Write(NewMaxSize);
        }
    }
}
