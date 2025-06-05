using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberPromoted)]
    public class ServerGroupMemberPromoted : IWritable
    {
        public ulong GroupId { get; set; }
        public uint LeaderIndex { get; set; } // Unpacked but unused by Client
        public Identity NewLeader { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(LeaderIndex);
            NewLeader.Write(writer);
        }
    }
}
