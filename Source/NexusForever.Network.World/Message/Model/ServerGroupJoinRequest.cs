using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupJoinRequest)]
    public class ServerGroupJoinRequest : IWritable
    {
        public ulong GroupId { get; set; }
        public GroupMemberInfo JoinRequester { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            JoinRequester.Write(writer);
        }
    }
}
