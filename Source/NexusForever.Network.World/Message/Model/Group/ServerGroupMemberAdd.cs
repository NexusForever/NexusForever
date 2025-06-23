using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberAdd)]
    public class ServerGroupMemberAdd : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unused { get; set; } // Unpacked but not used by client
        public GroupMemberInfo AddedMemberInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unused);
            AddedMemberInfo.Write(writer);
        }
    }
}
