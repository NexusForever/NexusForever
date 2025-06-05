using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMatchingPenaltyUpdated)]
    public class ServerGroupMatchingPenaltyUpdated : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unused { get; set; } // Unpacked but unused by Client
        public Identity GroupMemberIdentity { get; set; }
        public ulong[] PenaltyTimeRemaining { get; set; } = new ulong[8];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unused);
            GroupMemberIdentity.Write(writer);
            foreach(var penaltyTime in PenaltyTimeRemaining)
            {
                writer.Write(penaltyTime);
            }
        }
    }
}
