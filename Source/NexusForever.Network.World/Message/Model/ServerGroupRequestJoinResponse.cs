using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupRequestJoinResponse)]
    public class ServerGroupRequestJoinResponse : IWritable
    {
        public ulong GroupId { get; set; }

        public GroupMemberInfo MemberInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            MemberInfo.Write(writer);
        }
    }
}
