using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberPrimeLevelUpdate)]
    public class ServerGroupMemberPrimeLevelUpdate : IWritable
    {
        public class PrimeLevelInfo : IWritable
        {
            public ushort WorldId { get; set; }
            public ushort PrimeLevelAchieved { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(WorldId, 15);
                writer.Write(PrimeLevelAchieved);
            }
        }

        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public List<PrimeLevelInfo> PrimeLeveInfo { get; set; } = new List<PrimeLevelInfo>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            Identity.Write(writer);
            writer.Write(PrimeLeveInfo.Count);
            foreach (var info in PrimeLeveInfo)
            {
                info.Write(writer);
            }
        }
    }
}
