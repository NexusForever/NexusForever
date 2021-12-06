using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class GroupMemberInfo : IWritable
    {
        public TargetPlayerIdentity MemberIdentity { get; set; } = new TargetPlayerIdentity();
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
