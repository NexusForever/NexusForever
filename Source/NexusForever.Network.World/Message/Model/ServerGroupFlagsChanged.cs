using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupFlagsChanged)]
    public class ServerGroupFlagsChanged : IWritable
    {
        public ulong GroupId { get; set; }

        public GroupFlags Flags { get; set; }

        public uint Unk0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unk0);
            writer.Write(Flags, 32u);
        }
    }
}
