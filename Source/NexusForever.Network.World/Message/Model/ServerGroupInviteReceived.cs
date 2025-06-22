using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupInviteReceived)]
    public class ServerGroupInviteReceived : IWritable
    {
        public ulong GroupId { get; set; }
        public uint LeaderIndex { get; set; }
        public uint InviterIndex { get; set; }

        public List<GroupMember> Members = new List<GroupMember>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(LeaderIndex);
            writer.Write(InviterIndex);

            writer.Write(Members.Count);
            Members.ForEach(x => x.Write(writer));
        }
    }
}
