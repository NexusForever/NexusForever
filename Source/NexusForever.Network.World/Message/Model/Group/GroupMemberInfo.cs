using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Group
{
    public class GroupMemberInfo : IWritable
    {
        public Identity MemberIdentity { get; set; } = new();
        public GroupMemberInfoFlags Flags { get; set; }
        public GroupMember Member { get; set; }
        public uint GroupIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            MemberIdentity.Write(writer);
            writer.Write(Flags, 32);
            Member.Write(writer);
            writer.Write(GroupIndex);
        }
    }
}
