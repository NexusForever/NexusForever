using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupFlagsChanged)]
    public class ServerGroupFlagsChanged : IWritable
    {
        public ulong GroupId { get; set; }
        public GroupFlags Flags { get; set; }
        public uint Unused { get; set; } // Unpacked but unused by Client

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unused);
            writer.Write(Flags, 32u);
        }
    }
}
