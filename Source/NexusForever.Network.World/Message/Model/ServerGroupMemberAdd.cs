using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberAdd)]
    public class ServerGroupMemberAdd : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unknown0 { get; set; }
        public GroupMember AddedMember { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unknown0);
            AddedMember.Write(writer);
        }
    }
}
