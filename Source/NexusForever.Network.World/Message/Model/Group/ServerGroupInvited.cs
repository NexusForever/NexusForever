using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupInvited)]
    public class ServerGroupInvited : IWritable
    {
        public ulong InviteId { get; set; }
        public uint LeaderIndex { get; set; } 
        public uint InviterIndex { get; set; }
        public List<GroupMember> Members = new List<GroupMember>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(InviteId);
            writer.Write(LeaderIndex);
            writer.Write(InviterIndex);

            writer.Write(Members.Count);
            Members.ForEach(x => x.Write(writer));
        }
    }
}
