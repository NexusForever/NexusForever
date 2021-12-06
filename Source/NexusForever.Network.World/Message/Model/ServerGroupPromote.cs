using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupPromote)]
    public class ServerGroupPromote : IWritable
    {
        public ulong GroupId { get; set; }

        public uint LeaderIndex { get; set; }

        public TargetPlayerIdentity NewLeader { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(LeaderIndex);
            NewLeader.Write(writer);
        }
    }
}
