using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberFlagsChanged)]
    public class ServerGroupMemberFlagsChanged : IWritable
    {
        public ulong GroupId { get; set; }
        public uint GroupMessageIndex { get; set; } // Unpacked but unused by Client
        public Identity MemberIdentity { get; set; } = new Identity();
        public GroupMemberInfoFlags ChangedFlags { get; set; }
        public bool IsFromPromotion { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(GroupMessageIndex);
            MemberIdentity.Write(writer);
            writer.Write(ChangedFlags, 32);
            writer.Write(IsFromPromotion);
        }
    }
}
